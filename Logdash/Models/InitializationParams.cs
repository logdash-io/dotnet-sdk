using Logdash.Constants;

namespace Logdash.Models;

/// <summary>
/// Settings for the Logdash client.
/// </summary>
/// <param name="ApiKey">Key that authorizes requests to the Logdash API: a domain key or an older service key.</param>
/// <param name="Host">Custom API host, useful with self-hosted instances.</param>
/// <param name="Verbose">Prints metric changes to the console, useful for debugging.</param>
/// <param name="App">
/// Name of the app that sends the logs and metrics, for example <c>desktop</c> or <c>api</c>, so the dashboard can filter by app.
/// The domain key is shared by all apps of a domain; this setting names this one.
/// Older service keys ignore it.
/// When unset or blank, no app is sent.
/// </param>
/// <example>
/// <code>
/// builder.Services.AddLogdash(new InitializationParams("INSERT_DOMAIN_KEY", App: "api"));
/// </code>
/// </example>
public record InitializationParams(string? ApiKey, string? Host = LogdashConstants.LogdashApiUrl, bool Verbose = LogdashConstants.DefaultVerbose, string? App = null);