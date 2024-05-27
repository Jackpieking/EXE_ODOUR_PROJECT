using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ODour.FastEndpointApi.ServiceConfigs;

/// <summary>
///     Core service config.
/// </summary>
internal static class CoreServiceConfig
{
    internal static void ConfigCore(
        this IServiceCollection services,
        IConfiguration configuration
    ) { }
}
