using Microsoft.Extensions.DependencyInjection;
using ODour.AppIdentityService.ServiceConfigs;

namespace ODour.AppIdentityService;

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
    public static void AddAppIdentityService(this IServiceCollection services)
    {
        CoreServiceConfig.Config(services: services);
    }
}
