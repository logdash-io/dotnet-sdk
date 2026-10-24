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

        provider.GetRequiredService<ILogdashEvents>().Track("agent_run");
        var @event = await handler.NextBody(HttpMethod.Post, "/events");

        Assert.Equal("api", (string?)log["app"]);
        Assert.Equal("api", (string?)metric["app"]);
        Assert.Equal("api", (string?)@event["app"]);
    }

    private static LogdashClient Build(HttpMessageHandler handler, string? app) =>
        new LogdashBuilder()
            .WithHttpClient(new HttpClient(handler))
            .WithInitializationParams(new InitializationParams("domain-key", App: app))
            .Build();
}
