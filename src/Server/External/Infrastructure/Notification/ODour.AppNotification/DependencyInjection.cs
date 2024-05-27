using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ODour.AppNotification.ServiceConfigs;

namespace ODour.AppNotification;

public static class DependencyInjection
{
    public static void AddAppNotification(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        CoreServiceConfig.AddCore(services: services, configuration: configuration);
    }
}
