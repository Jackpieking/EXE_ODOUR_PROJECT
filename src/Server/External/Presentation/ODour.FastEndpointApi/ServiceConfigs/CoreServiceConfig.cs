using System.Security.Cryptography;
using System.Text;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ODour.Application.Share.Common;
using ODour.Configuration.Infrastructure.Mail.GoogleGmail;
using ODour.Configuration.Presentation.WebApi.Authentication;
using ODour.Configuration.Presentation.WebApi.SecurityKey;
using ODour.FastEndpointApi.Shared.Authorization;

namespace ODour.FastEndpointApi.ServiceConfigs;

/// <summary>
///     Core service config.
/// </summary>
internal static class CoreServiceConfig
{
    internal static void Config(IServiceCollection services, IConfiguration configuration)
    {
        var option = configuration
            .GetRequiredSection(key: "Authentication")
            .Get<JwtAuthenticationOption>();

        // Add config from json.
        services
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
            .MakeSingletonLazy<AdminAccessSecurityKeyOption>()
            .AddSingleton(
                new TokenValidationParameters
                {
                    ValidateIssuer = option.Jwt.ValidateIssuer,
                    ValidateAudience = option.Jwt.ValidateAudience,
                    ValidateLifetime = option.Jwt.ValidateLifetime,
                    ValidateIssuerSigningKey = option.Jwt.ValidateIssuerSigningKey,
                    RequireExpirationTime = option.Jwt.RequireExpirationTime,
                    ValidTypes = option.Jwt.ValidTypes,
                    ValidIssuer = option.Jwt.ValidIssuer,
                    ValidAudience = option.Jwt.ValidAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        key: new HMACSHA256(
                            key: Encoding.UTF8.GetBytes(s: option.Jwt.IssuerSigningKey)
                        ).Key
                    )
                }
            )
            .MakeSingletonLazy<TokenValidationParameters>()
            .AddSingleton(implementationInstance: option)
            .MakeSingletonLazy<JwtAuthenticationOption>();

        // Config other services.
        services
            .MakeSingletonLazy<IDataProtectionProvider>()
            .MakeSingletonLazy<IServiceScopeFactory>();

        // Authorization.
        services.AddSingleton<
            IDashboardAuthorizationFilter,
            HangfireDashboardAdminKeyAuthorizationFilter
        >();
    }
}
