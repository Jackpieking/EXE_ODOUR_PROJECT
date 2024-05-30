using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ODour.FastEndpointApi.ServiceConfigs;

/// <summary>
///     Logging service config.
/// </summary>
internal static class LoggingServiceConfig
{
    internal static void Config(IServiceCollection services)
    {
        services.AddLogging(configure: config =>
        {
            config.ClearProviders();
            config.AddConsole();
        });
    }
}
