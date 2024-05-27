using Microsoft.Extensions.DependencyInjection;
using ODour.AppIdentityService.Handlers;
using ODour.Application.Share.Tokens.AccessToken;
using ODour.Application.Share.Tokens.RefreshToken;

namespace ODour.AppIdentityService.ServiceConfigs;

/// <summary>
///     Core service config.
/// </summary>
internal static class CoreServiceConfig
{
    internal static void ConfigCore(IServiceCollection services)
    {
        services
            .AddSingleton<IAccessTokenHandler, AccessTokenHandler>()
            .AddSingleton<IRefreshTokenHandler, RefreshTokenHandler>();
    }
}
