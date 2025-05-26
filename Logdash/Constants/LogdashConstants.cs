using Logdash.Models;

namespace Logdash.Constants;

public static class LogdashConstants
{
    public const string LogdashApiUrl = "https://api.logdash.com";
    public const bool DefaultVerbose = false;

    public static readonly Dictionary<LogLevel, (int r, int g, int b)> LogColorMap = new()
    {
        { LogLevel.Error, (231, 0, 11) },
        { LogLevel.Warn, (254, 154, 0) },
        { LogLevel.Info, (21, 93, 252) },
        { LogLevel.Http, (0, 166, 166) },
        { LogLevel.Verbose, (0, 166, 0) },
        { LogLevel.Debug, (0, 166, 62) },
        { LogLevel.Silly, (80, 80, 80) }
    };
}