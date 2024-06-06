using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ODour.FastEndpointApi.ServiceConfigs;

internal static class AppNotificationServiceConfig
{
    internal static IServiceCollection Config(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        return services;
    }
}
