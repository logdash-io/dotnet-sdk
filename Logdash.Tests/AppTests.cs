using System.Net;
using System.Text.Json.Nodes;
using System.Threading.Channels;
using Logdash.Extensions;
using Logdash.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Logdash.Tests;

public class AppTests
{
    [Fact]
    public async Task LogsCarryTheApp()
    {
        var handler = new RecordingHandler();
        var (logdash, _) = Build(handler, "desktop");

        logdash.Info("hello");

        var log = await handler.NextBody(HttpMethod.Post, "/logs");
        Assert.Equal("desktop", (string?)log["app"]);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public async Task LogsOmitTheAppWhenUnset(string? app)
    {
        var handler = new RecordingHandler();
        var (logdash, _) = Build(handler, app);

        logdash.Info("hello");

        var log = await handler.NextBody(HttpMethod.Post, "/logs");
        Assert.False(log.ContainsKey("app"));
    }

    [Fact]
    public async Task MetricsCarryTheApp()
    {
        var handler = new RecordingHandler();
        var (_, metrics) = Build(handler, "desktop");

        metrics.Set("users", 1);
        metrics.Mutate("users", 2);

        var bodies = await handler.NextSetAndMutateBodies();
        Assert.All(bodies, metric => Assert.Equal("desktop", (string?)metric["app"]));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public async Task MetricsOmitTheAppWhenUnset(string? app)
    {
        var handler = new RecordingHandler();
        var (_, metrics) = Build(handler, app);

        metrics.Set("users", 1);
        metrics.Mutate("users", 2);

        var bodies = await handler.NextSetAndMutateBodies();
        Assert.All(bodies, metric => Assert.False(metric.ContainsKey("app")));
    }

    [Fact]
    public async Task AddLogdashCarriesTheApp()
    {
        var handler = new RecordingHandler();
        using var provider = new ServiceCollection()
            .AddLogdash(new InitializationParams("domain-key", App: "api"))
            .ConfigureHttpClientDefaults(http => http.ConfigurePrimaryHttpMessageHandler(() => handler))
            .BuildServiceProvider();

        provider.GetRequiredService<ILogdashLogger>().Info("hello");
        var log = await handler.NextBody(HttpMethod.Post, "/logs");

        provider.GetRequiredService<ILogdashMetrics>().Set("users", 1);
        var metric = await handler.NextBody(HttpMethod.Put, "/metrics");

        Assert.Equal("api", (string?)log["app"]);
        Assert.Equal("api", (string?)metric["app"]);
    }

    private static (ILogdashLogger logdash, ILogdashMetrics metrics) Build(HttpMessageHandler handler, string? app) =>
        new LogdashBuilder()
            .WithHttpClient(new HttpClient(handler))
            .WithInitializationParams(new InitializationParams("domain-key", App: app))
            .Build();

    // Stands in for the Logdash API and records the JSON body of every request the SDK sends.
    private sealed class RecordingHandler : HttpMessageHandler
    {
        private readonly Channel<Request> _requests = Channel.CreateUnbounded<Request>();

        // The SDK sends in the background, so this waits for the next request.
        public async Task<JsonObject> NextBody(HttpMethod method, string path)
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var request = await _requests.Reader.ReadAsync(timeout.Token);

            Assert.Equal(method, request.Method);
            Assert.Equal(path, request.Path);
            return request.Body;
        }

        // Set and Mutate send concurrently, so this takes both of their bodies in either order.
        public async Task<JsonObject[]> NextSetAndMutateBodies()
        {
            JsonObject[] bodies = [await NextBody(HttpMethod.Put, "/metrics"), await NextBody(HttpMethod.Put, "/metrics")];

            Assert.Equal(new[] { "change", "set" }, bodies.Select(body => (string?)body["operation"]).Order());
            return bodies;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var body = JsonNode.Parse(await request.Content!.ReadAsStringAsync(cancellationToken))!.AsObject();
            _requests.Writer.TryWrite(new Request(request.Method, request.RequestUri!.AbsolutePath, body));
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        private sealed record Request(HttpMethod Method, string Path, JsonObject Body);
    }
}
