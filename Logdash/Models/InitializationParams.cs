using Logdash.Constants;

namespace Logdash.Models;

public record InitializationParams(string? ApiKey, string? Host = LogdashConstants.LogdashApiUrl, bool Verbose = LogdashConstants.DefaultVerbose);