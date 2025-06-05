using Logdash.Constants;
using Logdash.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Logdash.Extensions;

public static class LogdashExtensions
{
    public static IServiceCollection AddLogdash(this IServiceCollection serviceCollection, InitializationParams? initParams = null)
    {
        var requiredParams = new InitializationParams(initParams?.ApiKey ?? string.Empty,
            initParams?.Host ?? LogdashConstants.LogdashApiUrl,
            initParams?.Verbose ?? false);

        serviceCollection.AddSingleton(requiredParams);
        serviceCollection.AddHttpClient<Logdash>(x =>
        {
            x.BaseAddress = new Uri(requiredParams.Host!);
            x.DefaultRequestHeaders.Add("project-api-key", requiredParams.ApiKey);
        })
            .AddTypedClient<ILogdash>((_, provider) => provider.GetRequiredService<Logdash>())
            .AddTypedClient<ILogdashMetrics>((_, provider) => provider.GetRequiredService<Logdash>());

        return serviceCollection;
    }
}
