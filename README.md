# logdash - .NET SDK

Logdash is a zero-config observability platform. This package serves an javascript interface to use it.

## Pre-requisites

Setup your free project in less than 2 minutes at [logdash.io](https://logdash.io/)

## Installation

```
dotnet add package Logdash
```

## Setup

There are 2 ways to register Logdash depending on use-case. The most common one is for AspNetCore using it's Dependency Injection.
You can consult Example programs attached to this repo, or continue reading for quick recap.

### Setup - AspNetCore

```csharp
using Logdash;
using Logdash.Models;

builder.Services.AddLogdash(new InitializationParams("INSERT_API_KEY"));
```

Now you can receive your Logdash in Controllers or Services via DI:

```csharp
public class InfoService(ILogdash logdash, ILogdashMetrics metrics)
{
    /// ...
}
```

### Setup - Without AspNetCore DI
```csharp
using Logdash;
using Logdash.Models;

var builder = new LogdashBuilder();
var (logdash, metrics) = builder.WithHttpClient(new HttpClient())
    .WithInitializationParams(new InitializationParams("INSERT_API_KEY"))
    .Build();
```

## Logging and Metrics

After you're done with the setup, you can start logging or using metrics using Logdash

### Logging

```csharp
logdash.Debug("This is a debug message");
logdash.Error("This is an error message");
logdash.Info("This is info message");
logdash.Verbose("This is verbose message");
logdash.Http("This is http message");
logdash.Silly("This is silly message");
logdash.Warn("This is warn message");

logdash.Info("Hello", "From", "LogDash");
```

### Metrics

```csharp
metrics.SetMetric("key", 2);
metrics.MutateMetric("key", 3);
```

## Configuration

| Parameter | Required | Default | Description                                                                                                              |
| --------- | -------- | ------- | ------------------------------------------------------------------------------------------------------------------------ |
| `apiKey`  | no       | -       | Api key used to authorize against logdash servers. If you don't provide one, logs will be logged into local console only |
| `host`    | no       | -       | Custom API host, useful with self-hosted instances                                                                       |
| `verbose` | no       | -       | Useful for debugging purposes                                                                                            |

## License

This project is licensed under the MIT License.

## Contributing

Contributions are welcome! Feel free to open issues or submit pull requests.

## Support

If you encounter any issues, please open an issue on GitHub or let us know at [contact@logdash.io](mailto:contact@logdash.io).