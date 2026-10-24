# logdash - .NET SDK

Logdash is a zero-config observability platform. This package serves a .NET interface to use it.

## Pre-requisites

Setup your free project in less than 2 minutes at [logdash.io](https://logdash.io/)

## Installation

```
dotnet add package Logdash
```

## Setup

There are 2 ways to register Logdash depending on use-case. The most common one is for AspNetCore using its Dependency Injection.
You can consult Example programs attached to this repo, or continue reading for quick recap.

### Setup - AspNetCore

```csharp
using Logdash.Extensions;
using Logdash.Models;

builder.Services.AddLogdash(new InitializationParams("INSERT_API_KEY"));
```

Now you can receive your Logdash in Controllers or Services via DI:

```csharp
public class InfoService(ILogdashLogger logdash, ILogdashMetrics metrics, ILogdashEvents events)
{
    /// ...
}
```

### Setup - Without AspNetCore DI
```csharp
using Logdash;
using Logdash.Models;

var builder = new LogdashBuilder();
var (logdash, metrics, events) = builder.WithHttpClient(new HttpClient())
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
// to set absolute value
metrics.Set("key", 2);

// or increment / decrement by
metrics.Mutate("key", 3);
```

## Events

```csharp
events.Track("agent_run", new Dictionary<string, object>
{
    ["agent"] = "support",
    ["steps"] = 4,
    ["success"] = true,
}, userId: user.Id);
```

Events show up in the domain's Analytics as goals, with a daily trend.
Pass `userId` for signed-in users, so one person counts once across the website and apps.
Without it, Logdash counts a visitor of that app that changes every day.
Never put personal data in props.

## Configuration

| Parameter | Required | Default | Description                                                                                                                                   |
| --------- | -------- | ------- | --------------------------------------------------------------------------------------------------------------------------------------------- |
| `apiKey`  | no       | -       | Api key used to authorize against logdash servers. If you don't provide one, logs will be logged into local console only, and nothing is sent |
| `host`    | no       | -       | Custom API host, useful with self-hosted instances                                                                                            |
| `verbose` | no       | -       | Useful for debugging purposes                                                                                                                 |
| `app`     | no       | -       | Name of the app that sends the logs, metrics and events, see [App](#app)                                                                      |

### App

The domain key is shared by all apps of a domain, for example a desktop app, an API and a landing page.
The `App` setting names this one, so the dashboard can filter logs, metrics and events by app:

```csharp
builder.Services.AddLogdash(new InitializationParams("INSERT_DOMAIN_KEY", App: "api"));
```

Older service keys ignore `App`.

## License

This project is licensed under the MIT License.

## Contributing

Contributions are welcome! Feel free to open issues or submit pull requests.

## Support

If you encounter any issues, please open an issue on GitHub or let us know at [contact@logdash.io](mailto:contact@logdash.io).
