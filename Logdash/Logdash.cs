using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Logdash.Constants;
using Logdash.Extensions;
using Logdash.Models;
using Logdash.Requests;

namespace Logdash;

internal partial class Logdash(HttpClient httpClient, InitializationParams initializationParams) : ILogdashLogger, ILogdashMetrics, ILogdashEvents
{
    private readonly string? _app = string.IsNullOrWhiteSpace(initializationParams.App) ? null : initializationParams.App;
    private readonly bool _sends = !string.IsNullOrWhiteSpace(initializationParams.ApiKey);
    private int _sequenceNumber;

    public void Set(string key, double value)
    {
        if (initializationParams.Verbose)
        {
            Console.WriteLine($"Setting metric {key} to {value}");
        }

        Send(HttpMethod.Put, "/metrics", JsonSerializer.Serialize(new MetricRequest(key, value, MetricOperation.Set, _app), JsonSerializerOptions.Web));
    }

    public void Mutate(string key, double value)
    {
        if (initializationParams.Verbose)
        {
            Console.WriteLine($"Mutating metric {key} to {value}");
        }

        Send(HttpMethod.Put, "/metrics", JsonSerializer.Serialize(new MetricRequest(key, value, MetricOperation.Change, _app), JsonSerializerOptions.Web));
    }

    public void Track(string name, IReadOnlyDictionary<string, object>? props = null, string? userId = null)
    {
        if (name is null || !EventName().IsMatch(name) || name is "pageview" or "pageleave" or "browser_error")
        {
            PrintWarning($"Logdash dropped event \"{name}\": its name must match ^[a-z][a-z0-9_]{{0,63}}$ and not be pageview, pageleave or browser_error");
            return;
        }

        if (userId?.Contains('@') == true)
        {
            PrintWarning($"Logdash dropped event \"{name}\": its user ID must be an internal ID, never an email address");
            return;
        }

        if (initializationParams.Verbose)
        {
            Console.WriteLine($"Tracking event {name}");
        }

        // One id per event: a retried request resends it, so the server stores the event once.
        var request = new EventRequest(name, props, string.IsNullOrWhiteSpace(userId) ? null : userId, Guid.NewGuid(), _app);
        string json;
        try
        {
            json = JsonSerializer.Serialize(request, JsonSerializerOptions.Web);
        }
        catch
        {
            // Like the server, keep the event when its props are invalid, such as a NaN value.
            PrintWarning($"Logdash sent event \"{name}\" without its props: their values must be strings, finite numbers or booleans");
            json = JsonSerializer.Serialize(request with { Props = null }, JsonSerializerOptions.Web);
        }

        Send(HttpMethod.Post, "/events", json);
    }

    public void Debug(params object[] data) => Log(LogLevel.Debug, data);
    public void Error(params object[] data) => Log(LogLevel.Error, data);
    public void Info(params object[] data) => Log(LogLevel.Info, data);
    public void Verbose(params object[] data) => Log(LogLevel.Verbose, data);
    public void Http(params object[] data) => Log(LogLevel.Http, data);
    public void Silly(params object[] data) => Log(LogLevel.Silly, data);
    public void Warn(params object[] data) => Log(LogLevel.Warn, data);

    private void Log(LogLevel level, params object[] data)
    {
        var formattedItems = new List<string>();
        foreach (var item in data ?? [])
        {
            if (item is not (null or string) && item.GetType().IsClass)
            {
                try
                {
                    var js = JsonSerializer.Serialize(item, JsonSerializerOptions.Web);
                    formattedItems.Add(js);
                }
                catch
                {
                    formattedItems.Add(item.ToString() ?? string.Empty);
                }
            }
            else
            {
                formattedItems.Add(item?.ToString() ?? string.Empty);
            }
        }

        var dataString = string.Join(" ", formattedItems);

        var createdAt = DateTime.UtcNow.ToString("o");
        Print(level, createdAt, dataString);
        Send(HttpMethod.Post, "/logs", JsonSerializer.Serialize(new LogRequest(dataString, level, createdAt , _sequenceNumber++, _app), JsonSerializerOptions.Web));
    }

    // Sends in the background, so the app never waits for Logdash or sees its errors.
    // Without an API key nothing is sent, and logs are only printed to the console.
    private void Send(HttpMethod method, string path, string json)
    {
        if (!_sends)
        {
            return;
        }

        Task.Run(async () =>
        {
            try
            {
                using var request = new HttpRequestMessage(method, path);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                using var response = await httpClient.SendAsync(request);
            }
            catch
            {
                // Dropped, like a request the server rejects.
            }
        });
    }

    private static void Print(LogLevel level, string createdAt, string message)
    {
        var (r,g,b) = LogdashConstants.LogColorMap[level];
        var datePrefix = createdAt.WithColor(156, 156, 156);
        var coloredLevel = level.ToString().ToUpper().WithColor(r, g, b);

        Console.WriteLine($"[{datePrefix}] {coloredLevel} {message}");
    }

    // Printed like a warning log, but never sent as a log of the app.
    private static void PrintWarning(string message) => Print(LogLevel.Warn, DateTime.UtcNow.ToString("o"), message);

    // \z, because $ also matches before a trailing newline.
    [GeneratedRegex(@"^[a-z][a-z0-9_]{0,63}\z")]
    private static partial Regex EventName();
}