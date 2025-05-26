using Logdash.Models;

namespace Logdash;

public interface ILogdash
{
    void SetMetric(string key, double value);
    void MutateMetric(string key, double value);

    void Log(LogLevel level, params object[] data);
}