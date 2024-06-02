using Microsoft.Extensions.DependencyInjection;
using ODour.Application.Share.Common;
using ODour.Application.Share.Mail;
using ODour.AppNotification.Handlers;

namespace ODour.AppNotification.ServiceConfigs;

/// <summary>
///     Core service config.
/// </summary>
internal static class CoreServiceConfig
{
    internal static void Config(IServiceCollection services)
    {
        services
            .AddSingleton<ISendingMailHandler, GoogleSendingMailHandler>()
            .MakeSingletonLazy<ISendingMailHandler>();
    }
}
