using Microsoft.Extensions.DependencyInjection;

namespace ODour.FastEndpointApi.ServiceConfigs;

/// <summary>
///     Response caching service config.
/// </summary>
internal static class ResponseCachingServiceConfig
{
    internal static void ConfigResponseCaching(this IServiceCollection services)
    {
        services.AddResponseCaching();
    }
}
