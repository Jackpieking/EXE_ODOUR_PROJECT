using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ODour.RedisCacheDb.ServiceConfigs;

namespace ODour.RedisCacheDb;

/// <summary>
///     Configure services for this layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    ///     Entry to configuring multiple services.
    /// </summary>
    /// <param name="services">
    ///     Service container.
    /// </param>
    /// <param name="configuration">
    ///     Load configuration for configuration
    ///     file (appsetting).
    /// </param>
    public static void AddRedisCachingDatabase(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        CoreServiceConfig.ConfigC(services: services);
        StackExchangeRedisCacheServiceConfig.Config(
            services: services,
            configuration: configuration
        );
    }
}
