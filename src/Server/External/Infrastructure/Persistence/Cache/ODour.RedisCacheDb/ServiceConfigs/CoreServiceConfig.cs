using Microsoft.Extensions.DependencyInjection;
using ODour.Application.Share.Caching;
using ODour.Application.Share.Common;
using ODour.RedisCacheDb.Handler;

namespace ODour.RedisCacheDb.ServiceConfigs;

/// <summary>
///     Core service config.
/// </summary>
internal static class CoreServiceConfig
{
    internal static void ConfigC(IServiceCollection services)
    {
        services.AddScoped<ICacheHandler, CacheHandler>();
        services.MakeScopedLazy<ICacheHandler>();
    }
}
