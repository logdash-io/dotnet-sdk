using Logdash.Models;
using Xunit;

namespace Logdash.Tests;

public class LogdashTests
{
    [Fact]
    public async Task LogsAcceptNull()
    {
        var handler = new RecordingHandler();
        var (logdash, _) = Build(handler, "domain-key");

        logdash.Info("user", null!);

        var log = await handler.NextBody(HttpMethod.Post, "/logs");
        Assert.Equal("user ", (string?)log["message"]);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task NothingIsSentWithoutAnApiKey(string? apiKey)
    {
        var handler = new RecordingHandler();
        var (logdash, metrics, events) = Build(handler, apiKey);
        var (sender, _) = Build(handler, "domain-key");

        logdash.Info("hello");
        metrics.Set("users", 1);
        events.Track("agent_run");
        sender.Info("sent");

        var request = await handler.Next(HttpMethod.Post, "/logs");
        Assert.Equal("sent", (string?)request.Body["message"]);
        Assert.Equal("domain-key", request.ApiKey);
        await handler.AssertNoMoreRequests();
    }

    private static LogdashClient Build(HttpMessageHandler handler, string? apiKey) =>
        new LogdashBuilder()
            .WithHttpClient(new HttpClient(handler))
            .WithInitializationParams(new InitializationParams(apiKey))
            .Build();
}
