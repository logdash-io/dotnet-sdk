using System.Text;
using System.Text.Json;
using Logdash.Models;
using Logdash.Requests;

namespace Logdash;

public class Logdash(HttpClient httpClient, InitializationParams initializationParams) : ILogdash
{
    private int _sequenceNumber = 0;
    
    public void SetMetric(string key, double value)
    {
        if (initializationParams.Verbose)
        {
            Console.WriteLine($"Setting metric {key} to {value}");
        }
        
        var json = JsonSerializer.Serialize(new MetricRequest(key, value, MetricOperation.Set), JsonSerializerOptions.Web);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        Task.Run(async () => await httpClient.PutAsync("/metrics", content));
    }

    public void MutateMetric(string key, double value)
    {
        if (initializationParams.Verbose)
        {
            Console.WriteLine($"Mutating metric {key} to {value}");
        }
        
        var json = JsonSerializer.Serialize(new MetricRequest(key, value, MetricOperation.Change), JsonSerializerOptions.Web);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        Task.Run(async () => await httpClient.PutAsync("/metrics", content));
    }

    public void Log(LogLevel level, params object[] data)
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
        
        var json = JsonSerializer.Serialize(new LogRequest(dataString, level, DateTime.UtcNow.ToString("o"), _sequenceNumber++), JsonSerializerOptions.Web);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        Task.Run(async () =>
        {
            await httpClient.PostAsync("/logs", content);
        });
    }
}