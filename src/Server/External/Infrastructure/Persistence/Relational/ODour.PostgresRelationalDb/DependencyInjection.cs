using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ODour.PostgresRelationalDb.ServiceConfigs;

namespace ODour.PostgresRelationalDb;

public static class DependencyInjection
{
    public static void AddOdourPostgresRelationalDb(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        CoreServiceConfig.Config(services: services);
        ODourMainDbContextServiceConfig.Config(
            services: services,
            configurationManager: configuration
        );
        IdentityServiceConfig.Config(services: services, configuration: configuration);
    }
}
