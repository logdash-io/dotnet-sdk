namespace Logdash;

public interface ILogdashMetrics
{
    void SetMetric(string key, double value);
    void MutateMetric(string key, double value);
}