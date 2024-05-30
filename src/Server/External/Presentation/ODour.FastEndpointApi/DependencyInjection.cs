using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ODour.FastEndpointApi.ServiceConfigs;

namespace ODour.FastEndpointApi;

public static class DependencyInjection
{
    internal static void AddWebApi(this IServiceCollection services, IConfiguration configuration)
    {
        AuthenticationServiceConfig.Config(services: services, configuration: configuration);
        AuthorizationServiceConfig.Config(services: services);
        LoggingServiceConfig.Config(services: services);
        CoreServiceConfig.Config(services: services, configuration: configuration);
        CorsServiceConfig.Config(services: services);
        SwaggerServiceConfig.Config(services: services, configuration: configuration);
        ResponseCachingServiceConfig.Config(services: services);
        DataProtectionServiceConfig.Config(services: services);
    }
}
