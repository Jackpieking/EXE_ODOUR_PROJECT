using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ODour.AppBackgroundJob.ServiceConfigs;

namespace ODour.AppBackgroundJob;

public static class DependencyInjection
{
    public static void AddAppBackgroundJob(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        CoreServiceConfig.Config(services: services);
        AppBackgroundJobServiceConfig.Config(services: services, configuration: configuration);
    }
}
