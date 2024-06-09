using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ODour.AppNotification;

public static class AppNotificationServiceConfig
{
    public static IServiceCollection Config(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        return services;
    }
}
