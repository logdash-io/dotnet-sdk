namespace Logdash;

public interface ILogdashMetrics
{
    void Set(string key, double value);
    void Mutate(string key, double value);
}