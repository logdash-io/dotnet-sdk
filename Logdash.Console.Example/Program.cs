// See https://aka.ms/new-console-template for more information

using Logdash;
using Logdash.Models;

var builder = new LogdashBuilder();
var logdash = builder.WithHttpClient(new HttpClient())
    .WithInitializationParams(new InitializationParams("INSERT_API_KEY"))
    .Build();

logdash.Log(LogLevel.Debug, "This is a debug message");
logdash.Log(LogLevel.Error, "This is an error message");
logdash.Log(LogLevel.Info, "This is info message");
logdash.Log(LogLevel.Verbose, "This is verbose message");
logdash.Log(LogLevel.Http, "This is http message");
logdash.Log(LogLevel.Silly, "This is silly message");
logdash.Log(LogLevel.Warn, "This is warn message");

logdash.Log(LogLevel.Info, "Hello", "From", "LogDash");

logdash.SetMetric("key", 2);
logdash.MutateMetric("key", 3);

await Task.Delay(5000);