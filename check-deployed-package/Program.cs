using Logdash;
using Logdash.Models;

Console.WriteLine("=== Logdash SDK Demo ===");

var apiKey = Environment.GetEnvironmentVariable("LOGDASH_API_KEY")!;
var logsSeed = Environment.GetEnvironmentVariable("LOGS_SEED") ?? "default";
var metricsSeed = Environment.GetEnvironmentVariable("METRICS_SEED") ?? "1";

Console.WriteLine("Using API Key: " + apiKey);
Console.WriteLine("Using Logs Seed " + logsSeed);
Console.WriteLine("Using Metrics Seed " + metricsSeed);

var builder = new LogdashBuilder();
var (logdash, metrics) = builder.WithHttpClient(new HttpClient())
    .WithInitializationParams(new InitializationParams("INSERT_API_KEY"))
    .Build();

logdash.Debug("This is a debug message", logsSeed);
logdash.Error("This is an error message", logsSeed);
logdash.Info("This is info message", logsSeed);
logdash.Verbose("This is verbose message", logsSeed);
logdash.Http("This is http message", logsSeed);
logdash.Silly("This is silly message", logsSeed);
logdash.Warn("This is warn message", logsSeed);

metrics.Set("users", int.Parse(metricsSeed));
metrics.Mutate("users", 1);

await Task.Delay(2000);
