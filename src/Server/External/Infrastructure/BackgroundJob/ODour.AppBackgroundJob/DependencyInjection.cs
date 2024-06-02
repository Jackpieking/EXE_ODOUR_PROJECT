using Microsoft.Extensions.DependencyInjection;
using ODour.AppBackgroundJob.ServiceConfigs;

namespace ODour.AppBackgroundJob;

public static class DependencyInjection
{
    public static void AddAppBackgroundJob(this IServiceCollection services)
    {
        CoreServiceConfig.Config(services: services);
    }
}
