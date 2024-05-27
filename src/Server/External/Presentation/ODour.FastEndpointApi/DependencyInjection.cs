using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ODour.FastEndpointApi.ServiceConfigs;

namespace ODour.FastEndpointApi;

public static class DependencyInjection
{
    internal static void AddWebApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigCore(configuration: configuration);
        services.ConfigAuthorization();
        services.ConfigLogging();
        services.ConfigCors();
        services.ConfigCore(configuration: configuration);
        services.ConfigSwagger(configuration: configuration);
        services.ConfigResponseCaching();
    }
}
