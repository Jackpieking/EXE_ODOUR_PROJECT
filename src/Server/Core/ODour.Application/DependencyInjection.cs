using Microsoft.Extensions.DependencyInjection;
using ODour.Application.Share.ServiceConfigs;

namespace ODour.Application;

/// <summary>
///     Entry to configuring multiple services
///     of the application.
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
    public static void AddApplication(this IServiceCollection services)
    {
        services.ConfigFastEndpoint();
    }
}
