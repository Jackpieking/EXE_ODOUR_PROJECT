using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ODour.Configuration.Infrastructure.Persistence.Cache.Redis;

namespace ODour.RedisCacheDb;

public static class RedisCachingDbServiceConfig
{
    public static IServiceCollection Config(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // ====
        var option = configuration
            .GetRequiredSection(key: "Cache")
            .GetRequiredSection(key: "Redis")
            .Get<RedisOption>();

        services.AddStackExchangeRedisCache(setupAction: config =>
        {
            config.Configuration = option.ConnectionString;
        });

        return services;
    }
}
