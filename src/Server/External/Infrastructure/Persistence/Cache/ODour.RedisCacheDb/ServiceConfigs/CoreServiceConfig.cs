using Microsoft.Extensions.DependencyInjection;
using ODour.Application.Share.Caching;
using ODour.RedisCacheDb.Handler;

namespace ODour.RedisCacheDb.ServiceConfigs;

/// <summary>
///     Core service config.
/// </summary>
internal static class CoreServiceConfig
{
    internal static void ConfigCore(IServiceCollection services)
    {
        services.AddScoped<ICacheHandler, CacheHandler>();
    }
}
