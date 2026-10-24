namespace Logdash;

/// <summary>
/// The logger, metrics and events that <see cref="LogdashBuilder.Build"/> returns.
/// </summary>
/// <example>
/// <code>
/// var (logdash, metrics, events) = new LogdashBuilder()
///     .WithHttpClient(new HttpClient())
///     .WithInitializationParams(new InitializationParams("INSERT_API_KEY"))
///     .Build();
/// </code>
/// </example>
public sealed record LogdashClient(ILogdashLogger Logger, ILogdashMetrics Metrics, ILogdashEvents Events)
{
    /// <summary>
    /// Keeps <c>var (logdash, metrics) = builder.Build();</c> working.
    /// </summary>
    public void Deconstruct(out ILogdashLogger logger, out ILogdashMetrics metrics) => (logger, metrics) = (Logger, Metrics);
}
