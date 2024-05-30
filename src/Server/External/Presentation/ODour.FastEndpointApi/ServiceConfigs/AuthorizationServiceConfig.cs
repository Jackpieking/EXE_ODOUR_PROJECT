using Microsoft.Extensions.DependencyInjection;

namespace ODour.FastEndpointApi.ServiceConfigs;

/// <summary>
///     Authorization service config.
/// </summary>
public static class AuthorizationServiceConfig
{
    internal static void Config(IServiceCollection services)
    {
        services.AddAuthorization();
    }
}
