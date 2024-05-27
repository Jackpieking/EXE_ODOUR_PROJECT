using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;

namespace ODour.Application.Share.ServiceConfigs;

/// <summary>
///     Fast endpoint service config.
/// </summary>
internal static class FastEndpointServiceConfig
{
    internal static void ConfigFastEndpoint(this IServiceCollection services)
    {
        services.AddFastEndpoints();
    }
}
