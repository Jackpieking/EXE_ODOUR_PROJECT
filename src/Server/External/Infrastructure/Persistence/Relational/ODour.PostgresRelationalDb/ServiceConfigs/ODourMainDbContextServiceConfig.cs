using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ODour.Configuration.Infrastructure.Persistence.Database;
using ODour.PostgresRelationalDb.Data;

namespace ODour.PostgresRelationalDb.ServiceConfigs;

internal static class ODourMainDbContextServiceConfig
{
    internal static void Config(IServiceCollection services, IConfiguration configurationManager)
    {
        services.AddDbContextPool<ODourContext>(
            optionsAction: (provider, config) =>
            {
                var option = configurationManager
                    .GetRequiredSection(key: "Database")
                    .GetRequiredSection(key: "ODourMainDb")
                    .Get<ODourDatabaseOption>();

                config
                    .UseNpgsql(
                        connectionString: option.ConnectionString,
                        npgsqlOptionsAction: npgsqlOptionsAction =>
                        {
                            npgsqlOptionsAction
                                .CommandTimeout(commandTimeout: option.CommandTimeOut)
                                .EnableRetryOnFailure(maxRetryCount: option.EnableRetryOnFailure)
                                .MigrationsAssembly(
                                    assemblyName: typeof(ODourContext).Assembly.FullName
                                );
                        }
                    )
                    .EnableSensitiveDataLogging(
                        sensitiveDataLoggingEnabled: option.EnableSensitiveDataLogging
                    )
                    .EnableDetailedErrors(detailedErrorsEnabled: option.EnableDetailedErrors)
                    .EnableThreadSafetyChecks(enableChecks: option.EnableThreadSafetyChecks)
                    .EnableServiceProviderCaching(
                        cacheServiceProvider: option.EnableServiceProviderCaching
                    );
            }
        );
    }
}
