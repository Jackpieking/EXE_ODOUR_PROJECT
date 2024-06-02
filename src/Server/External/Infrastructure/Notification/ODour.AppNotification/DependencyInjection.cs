using Microsoft.Extensions.DependencyInjection;
using ODour.AppNotification.ServiceConfigs;

namespace ODour.AppNotification;

public static class DependencyInjection
{
    public static void AddAppNotification(this IServiceCollection services)
    {
        CoreServiceConfig.Config(services: services);
    }
}
