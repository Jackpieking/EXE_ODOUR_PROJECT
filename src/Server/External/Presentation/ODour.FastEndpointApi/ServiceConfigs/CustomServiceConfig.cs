using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ODour.AppBackgroundJob.Handler;
using ODour.AppIdentityService.Handlers;
using ODour.Application.Share.BackgroundJob;
using ODour.Application.Share.Caching;
using ODour.Application.Share.Common;
using ODour.Application.Share.DataProtection;
using ODour.Application.Share.Mail;
using ODour.Application.Share.Session;
using ODour.Application.Share.Tokens;
using ODour.AppNotification.Handlers;
using ODour.Configuration.Infrastructure.Mail.GoogleGmail;
using ODour.Configuration.Presentation.WebApi.Authentication;
using ODour.Configuration.Presentation.WebApi.SecurityKey;
using ODour.Domain.Feature.Main;
using ODour.Domain.Share.Role.Entities;
using ODour.Domain.Share.User.Entities;
using ODour.PostgresRelationalDb.Main;
using ODour.RedisCacheDb.Handler;
using ODour.RedisSessionStorage.Handler;

namespace ODour.FastEndpointApi.ServiceConfigs;

internal static class CustomServiceConfig
{
    internal static IServiceCollection Config(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        #region Configs
        services
            // ====
            .AddSingleton(
                implementationInstance: configuration
                    .GetRequiredSection(key: "SmtpServer")
                    .GetRequiredSection(key: "GoogleGmail")
                    .Get<GoogleGmailSendingOption>()
            )
            .MakeSingletonLazy<GoogleGmailSendingOption>();

        // ====
        services
            .AddSingleton(
                implementationInstance: configuration
                    .GetRequiredSection(key: "SecurityKey")
                    .GetRequiredSection(key: "AppBaseProtection")
                    .Get<AppBaseProtectionSecurityKeyOption>()
            )
            .MakeSingletonLazy<AppBaseProtectionSecurityKeyOption>();

        // ====
        services
            .AddSingleton(
                implementationInstance: configuration
                    .GetRequiredSection(key: "SecurityKey")
                    .GetRequiredSection(key: "AdminAccess")
                    .Get<AdminAccessSecurityKeyOption>()
            )
            .MakeSingletonLazy<AdminAccessSecurityKeyOption>();

        // ====
        var authOption = configuration
            .GetRequiredSection(key: "Authentication")
            .Get<JwtAuthenticationOption>();

        services
            .AddSingleton(
                new TokenValidationParameters
                {
                    ValidateIssuer = authOption.Jwt.ValidateIssuer,
                    ValidateAudience = authOption.Jwt.ValidateAudience,
                    ValidateLifetime = authOption.Jwt.ValidateLifetime,
                    ValidateIssuerSigningKey = authOption.Jwt.ValidateIssuerSigningKey,
                    RequireExpirationTime = authOption.Jwt.RequireExpirationTime,
                    ValidTypes = authOption.Jwt.ValidTypes,
                    ValidIssuer = authOption.Jwt.ValidIssuer,
                    ValidAudience = authOption.Jwt.ValidAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        key: new HMACSHA256(
                            key: Encoding.UTF8.GetBytes(s: authOption.Jwt.IssuerSigningKey)
                        ).Key
                    )
                }
            )
            .MakeSingletonLazy<TokenValidationParameters>();

        // ====
        services
            .AddSingleton(implementationInstance: authOption)
            .MakeSingletonLazy<JwtAuthenticationOption>();
        #endregion

        #region Others
        // ====
        services.MakeScopedLazy<DbContext>();

        // ====
        services.AddScoped<IMainUnitOfWork, MainUnitOfWork>().MakeScopedLazy<IMainUnitOfWork>();

        // ====
        services.MakeScopedLazy<UserManager<UserEntity>>();

        // ====
        services.MakeScopedLazy<RoleManager<RoleEntity>>();

        // ====
        services.MakeScopedLazy<SignInManager<UserEntity>>();

        // ====
        services.MakeSingletonLazy<IDataProtectionProvider>();

        // ====
        services.MakeSingletonLazy<IServiceScopeFactory>();

        // ====
        services
            .AddSingleton<ICacheHandler, RedisCacheHandler>()
            .MakeSingletonLazy<ICacheHandler>();

        // ====
        services
            .AddSingleton<ISendingMailHandler, GoogleSendingMailHandler>()
            .MakeSingletonLazy<ISendingMailHandler>();

        // ====
        services
            .AddSingleton<IAccessTokenHandler, AppAccessTokenHandler>()
            .MakeSingletonLazy<IAccessTokenHandler>();

        // ====
        services
            .AddSingleton<IRefreshTokenHandler, AppRefreshTokenHandler>()
            .MakeSingletonLazy<IRefreshTokenHandler>();

        // ====
        services
            .AddSingleton<IDataProtectionHandler, AppDataProtectionHandler>()
            .MakeSingletonLazy<IDataProtectionHandler>();

        // ====
        services
            .AddSingleton<IAdminAccessKeyHandler, AppAdminAccessKeyHandler>()
            .MakeSingletonLazy<IAdminAccessKeyHandler>();

        // ====
        services
            .AddSingleton<IQueueHandler, FastEndpointQueueHandler>()
            .MakeSingletonLazy<IQueueHandler>();

        // ====
        services.MakeSingletonLazy<IHttpContextAccessor>();

        // ====
        services.AddScoped<IUserSession, UserSessionHandler>().MakeScopedLazy<IUserSession>();
        #endregion

        return services;
    }
}
