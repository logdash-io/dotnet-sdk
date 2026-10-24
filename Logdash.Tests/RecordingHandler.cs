using System.Net;
using System.Text.Json.Nodes;
using System.Threading.Channels;
using Xunit;

namespace Logdash.Tests;

// Stands in for the Logdash API and records every request the SDK sends.
internal sealed class RecordingHandler : HttpMessageHandler
{
    private readonly Channel<Request> _requests = Channel.CreateUnbounded<Request>();

    // The SDK sends in the background, so this waits for the next request.
    public async Task<Request> Next(HttpMethod method, string path)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var request = await _requests.Reader.ReadAsync(timeout.Token);

        Assert.Equal(method, request.Method);
        Assert.Equal(path, request.Path);
        return request;
    }

    public async Task<JsonObject> NextBody(HttpMethod method, string path) => (await Next(method, path)).Body;

    // A request the SDK started before the awaited one arrives within moments, so this waits briefly for any.
    public async Task AssertNoMoreRequests()
    {
        await Task.Delay(TimeSpan.FromMilliseconds(100));
        Assert.False(_requests.Reader.TryRead(out var request), $"Unexpected {request?.Method} {request?.Path}");
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
        var apiKey = request.Headers.TryGetValues("project-api-key", out var values) ? string.Join(", ", values) : null;
        _requests.Writer.TryWrite(new Request(request.Method, request.RequestUri!.AbsolutePath, apiKey,
            request.Content.Headers.ContentType?.MediaType, body));
        return new HttpResponseMessage(HttpStatusCode.OK);
    }

    public sealed record Request(HttpMethod Method, string Path, string? ApiKey, string? ContentType, JsonObject Body);
}
