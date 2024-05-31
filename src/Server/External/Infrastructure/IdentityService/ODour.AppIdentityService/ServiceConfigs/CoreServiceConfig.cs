using Microsoft.Extensions.DependencyInjection;
using ODour.AppIdentityService.Handlers;
using ODour.Application.Share.Common;
using ODour.Application.Share.DataProtection;
using ODour.Application.Share.Tokens.AccessToken;
using ODour.Application.Share.Tokens.RefreshToken;

namespace ODour.AppIdentityService.ServiceConfigs;

/// <summary>
///     Core service config.
/// </summary>
internal static class CoreServiceConfig
{
    internal static void Config(IServiceCollection services)
    {
        services
            .AddSingleton<IAccessTokenHandler, AppAccessTokenHandler>()
            .MakeSingletonLazy<IAccessTokenHandler>()
            // ====
            .AddSingleton<IRefreshTokenHandler, AppRefreshTokenHandler>()
            .MakeSingletonLazy<IRefreshTokenHandler>()
            // ====
            .AddSingleton<IDataProtectionHandler, AppDataProtectionHandler>()
            .MakeSingletonLazy<IDataProtectionHandler>();
    }
}
