using Logdash.Models;

namespace Logdash;

public interface ILogdash
{
    Task SetMetricAsync(string key, double value);
    Task MutateMetricAsync(string key, string value);

    Task LogAsync(LogLevel level, params object[] data);
}