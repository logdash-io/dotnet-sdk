using Logdash.Models;
using Xunit;

namespace Logdash.Tests;

// Some tests read the console, so no other test may write to it meanwhile.
[CollectionDefinition(nameof(EventTests), DisableParallelization = true)]
public class EventTestsCollection;

[Collection(nameof(EventTests))]
public class EventTests
{
    [Fact]
    public async Task TrackPostsTheEvent()
    {
        var handler = new RecordingHandler();
        var events = Build(handler).Events;

        events.Track("agent_run", new Dictionary<string, object> { ["model"] = "opus", ["tools"] = 3, ["cached"] = true });

        var request = await handler.Next(HttpMethod.Post, "/events");
        Assert.Equal("domain-key", request.ApiKey);
        Assert.Equal("application/json", request.ContentType);
        Assert.Equal(4, Guid.Parse((string)request.Body["id"]!).Version);
        request.Body.Remove("id");
        Assert.Equal("""{"name":"agent_run","props":{"model":"opus","tools":3,"cached":true}}""", request.Body.ToJsonString());
    }

    [Fact]
    public async Task EachEventGetsItsOwnId()
    {
        var handler = new RecordingHandler();
        var events = Build(handler).Events;

        events.Track("agent_run");
        events.Track("agent_run");

        var first = await handler.NextBody(HttpMethod.Post, "/events");
        var second = await handler.NextBody(HttpMethod.Post, "/events");
        Assert.NotEqual((string?)first["id"], (string?)second["id"]);
    }

    [Fact]
    public async Task EventsCarryTheApp()
    {
        var handler = new RecordingHandler();

        Build(handler, app: "desktop").Events.Track("agent_run");

        var body = await handler.NextBody(HttpMethod.Post, "/events");
        Assert.Equal("desktop", (string?)body["app"]);
    }

    [Fact]
    public async Task EventsCarryTheUserId()
    {
        var handler = new RecordingHandler();

        Build(handler).Events.Track("agent_run", userId: "42");

        var body = await handler.NextBody(HttpMethod.Post, "/events");
        Assert.Equal("42", (string?)body["userId"]);
    }

    [Theory]
    [InlineData("Agent run", null)]
    [InlineData("", null)]
    [InlineData("9lives", null)]
    [InlineData("agent_run\n", null)]
    [InlineData("pageview", null)]
    [InlineData(null, null)]
    [InlineData("agent_run", "alice@example.com")]
    public async Task InvalidEventsAreDroppedWithAWarning(string? name, string? userId)
    {
        var handler = new RecordingHandler();
        var events = Build(handler).Events;

        var console = ReadConsole(() => events.Track(name!, userId: userId));
        events.Track("next_event");

        Assert.Equal("next_event", (string?)(await handler.NextBody(HttpMethod.Post, "/events"))["name"]);
        await handler.AssertNoMoreRequests();
        Assert.Contains("dropped event", console);
        Assert.DoesNotContain("alice@example.com", console);
    }

    [Fact]
    public async Task EventsKeepGoingWithoutPropsThatCannotBeSerialized()
    {
        var handler = new RecordingHandler();
        var events = Build(handler).Events;

        var console = ReadConsole(() => events.Track("agent_run", new Dictionary<string, object> { ["ratio"] = double.NaN }));

        var body = await handler.NextBody(HttpMethod.Post, "/events");
        Assert.Equal("agent_run", (string?)body["name"]);
        Assert.False(body.ContainsKey("props"));
        Assert.Contains("without its props", console);
    }

    private static LogdashClient Build(HttpMessageHandler handler, string? app = null) =>
        new LogdashBuilder()
            .WithHttpClient(new HttpClient(handler))
            .WithInitializationParams(new InitializationParams("domain-key", App: app))
            .Build();

    private static string ReadConsole(Action action)
    {
        var console = Console.Out;
        var output = new StringWriter();
        Console.SetOut(output);
        try
        {
            action();
        }
        finally
        {
            Console.SetOut(console);
        }

        return output.ToString();
    }
}
