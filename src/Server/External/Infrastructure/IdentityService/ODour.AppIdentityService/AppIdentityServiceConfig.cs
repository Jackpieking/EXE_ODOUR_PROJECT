using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ODour.AppIdentityService;

public static class AppIdentityServiceConfig
{
    public static IServiceCollection Config(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        return services;
    }
}
