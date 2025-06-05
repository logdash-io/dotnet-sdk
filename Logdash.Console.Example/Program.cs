// See https://aka.ms/new-console-template for more information

using Logdash;
using Logdash.Models;

var builder = new LogdashBuilder();
var (logdash, metrics) = builder.WithHttpClient(new HttpClient())
    .WithInitializationParams(new InitializationParams("INSERT_API_KEY"))
    .Build();

logdash.Debug("This is a debug message");
logdash.Error("This is an error message");
logdash.Info("This is info message");
logdash.Verbose("This is verbose message");
logdash.Http("This is http message");
logdash.Silly("This is silly message");
logdash.Warn("This is warn message");

logdash.Info("Hello", "From", "LogDash");

metrics.Set("key", 2);
metrics.Mutate("key", 3);

await Task.Delay(5000);