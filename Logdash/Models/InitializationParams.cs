namespace Logdash.Models;

public record InitializationParams(string? ApiKey, string? Host = "https://api.logdash.io", bool Verbose = false);