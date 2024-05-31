using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ODour.Application.Share.Common;
using ODour.Configuration.Infrastructure.Mail.GoogleGmail;
using ODour.Configuration.Presentation.WebApi.Authentication;
using ODour.Configuration.Presentation.WebApi.SecurityKey;

namespace ODour.FastEndpointApi.ServiceConfigs;

/// <summary>
///     Core service config.
/// </summary>
internal static class CoreServiceConfig
{
    internal static void Config(IServiceCollection services, IConfiguration configuration)
    {
        var a = configuration
            .GetRequiredSection(key: "SecurityKey")
            .GetRequiredSection(key: "AppBaseProtection")
            .Get<AppBaseProtectionSecurityKeyOption>();

        // Add config from json.
        services
            .AddSingleton(
                implementationInstance: configuration
                    .GetRequiredSection(key: "Authentication")
                    .Get<JwtAuthenticationOption>()
            )
            .MakeSingletonLazy<JwtAuthenticationOption>()
            // ====
            .AddSingleton(
                implementationInstance: configuration
                    .GetRequiredSection(key: "SmtpServer")
                    .GetRequiredSection(key: "GoogleGmail")
                    .Get<GoogleGmailSendingOption>()
            )
            .MakeSingletonLazy<GoogleGmailSendingOption>()
            // ====
            .AddSingleton(
                implementationInstance: configuration
                    .GetRequiredSection(key: "SecurityKey")
                    .GetRequiredSection(key: "AppBaseProtection")
                    .Get<AppBaseProtectionSecurityKeyOption>()
            )
            .MakeSingletonLazy<AppBaseProtectionSecurityKeyOption>()
            // ====
            .AddSingleton(
                implementationInstance: configuration
                    .GetRequiredSection(key: "SecurityKey")
                    .GetRequiredSection(key: "AdminAccess")
                    .Get<AdminAccessSecurityKeyOption>()
            )
            .MakeSingletonLazy<AdminAccessSecurityKeyOption>();

        // Config other services.
        services.MakeSingletonLazy<IDataProtectionProvider>();
    }
}
