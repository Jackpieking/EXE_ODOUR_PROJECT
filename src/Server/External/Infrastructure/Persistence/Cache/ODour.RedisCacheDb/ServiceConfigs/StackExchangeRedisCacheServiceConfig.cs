using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ODour.Configuration.Infrastructure.Persistence.Cache.Redis;

namespace ODour.RedisCacheDb.ServiceConfigs;

/// <summary>
///     StackExchangeRedisCache service config.
/// </summary>
internal static class StackExchangeRedisCacheServiceConfig
{
    internal static void Config(IServiceCollection services, IConfiguration configuration)
    {
        var option = configuration
            .GetRequiredSection(key: "Cache")
            .GetRequiredSection(key: "Redis")
            .Get<RedisOption>();

        services.AddStackExchangeRedisCache(setupAction: config =>
        {
            config.Configuration = option.ConnectionString;
        });
    }
}
