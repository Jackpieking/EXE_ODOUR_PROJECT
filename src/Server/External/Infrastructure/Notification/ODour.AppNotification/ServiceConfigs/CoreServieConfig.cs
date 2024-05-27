using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ODour.Application.Share.Mail;
using ODour.AppNotification.Handlers;
using ODour.Configuration.Infrastructure.Mail.GoogleGmail;

namespace ODour.AppNotification.ServiceConfigs;

/// <summary>
///     Core service config.
/// </summary>
internal static class CoreServiceConfig
{
    internal static void AddCore(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddSingleton<ISendingMailHandler, GoogleSendingMailHandler>()
            .AddSingleton(
                configuration
                    .GetRequiredSection(key: "SmtpServer")
                    .GetRequiredSection(key: "GoogleGmail")
                    .Get<GoogleGmailSendingOption>()
            );
    }
}
