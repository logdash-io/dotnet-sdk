using Logdash;
using Logdash.Models;

Console.WriteLine("=== logdash SDK package check ===");

// Get LogDash package version
var version = typeof(LogdashBuilder).Assembly.GetName().Version?.ToString() ?? "unknown";
Console.WriteLine($"Using LogDash package version: {version}");
Console.WriteLine();

var apiKey = Environment.GetEnvironmentVariable("LOGDASH_API_KEY");
var logsSeed = Environment.GetEnvironmentVariable("LOGS_SEED") ?? "default";
var metricsSeed = Environment.GetEnvironmentVariable("METRICS_SEED") ?? "1";

Console.WriteLine($"Using API Key: {apiKey}");
Console.WriteLine($"Using Logs Seed: {logsSeed}");
Console.WriteLine($"Using Metrics Seed: {metricsSeed}");

var builder = new LogdashBuilder();
var (logdash, metrics) = builder.WithHttpClient(new HttpClient())
    .WithInitializationParams(new InitializationParams(apiKey))
    .Build();

// Log some messages with seed appended
logdash.Info("This is an info log", logsSeed);
logdash.Error("This is an error log", logsSeed);
logdash.Warn("This is a warning log", logsSeed);
logdash.Debug("This is a debug log", logsSeed);
logdash.Http("This is a http log", logsSeed);
logdash.Silly("This is a silly log", logsSeed);
logdash.Verbose("This is a verbose log", logsSeed);

// Set and mutate metrics with seed
metrics.Set("users", int.Parse(metricsSeed));
metrics.Mutate("users", 1);

await Task.Delay(5000); 