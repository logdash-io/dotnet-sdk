using System.Text;
using System.Text.Json;
using Logdash.Constants;
using Logdash.Extensions;
using Logdash.Models;
using Logdash.Requests;

namespace Logdash;

internal class Logdash(HttpClient httpClient, InitializationParams initializationParams) : ILogdashLogger, ILogdashMetrics
{
    private int _sequenceNumber;
    
    public void Set(string key, double value)
    {
        if (initializationParams.Verbose)
        {
            Console.WriteLine($"Setting metric {key} to {value}");
        }
        
        var json = JsonSerializer.Serialize(new MetricRequest(key, value, MetricOperation.Set), JsonSerializerOptions.Web);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        Task.Run(async () => await httpClient.PutAsync("/metrics", content));
    }

    public void Mutate(string key, double value)
    {
        if (initializationParams.Verbose)
        {
            Console.WriteLine($"Mutating metric {key} to {value}");
        }
        
        var json = JsonSerializer.Serialize(new MetricRequest(key, value, MetricOperation.Change), JsonSerializerOptions.Web);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        Task.Run(async () => await httpClient.PutAsync("/metrics", content));
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
        foreach (var item in data)
        {
            if (item.GetType().IsClass && item is not string)
            {
                try
                {
                    var js = JsonSerializer.Serialize(item, JsonSerializerOptions.Web);
                    formattedItems.Add(js);
                }
                catch
                {
                    formattedItems.Add(item.ToString());
                }
            }
            else
            {
                formattedItems.Add(item?.ToString() ?? string.Empty);
            }
        }

        var dataString = string.Join(" ", formattedItems);

        var createdAt = DateTime.UtcNow.ToString("o");
        var json = JsonSerializer.Serialize(new LogRequest(dataString, level, createdAt , _sequenceNumber++), JsonSerializerOptions.Web);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var (r,g,b) = LogdashConstants.LogColorMap[level];
        var datePrefix = createdAt.WithColor(156, 156, 156);
        var coloredLevel = level.ToString().ToUpper().WithColor(r, g, b);

        var formattedMessage = $"[{datePrefix}] {coloredLevel} {dataString}";
        Console.WriteLine(formattedMessage);
        
        Task.Run(async () =>
        {
            await httpClient.PostAsync("/logs", content);
        });
    }
}