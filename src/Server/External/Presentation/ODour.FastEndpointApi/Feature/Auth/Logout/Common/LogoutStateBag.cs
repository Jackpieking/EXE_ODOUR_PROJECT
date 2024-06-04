using ODour.Application.Feature.Auth.Logout;

namespace ODour.FastEndpointApi.Feature.Auth.Logout.Common;

internal sealed class LogoutStateBag
{
    internal string FoundRefreshTokenValue { get; set; }

    internal LogoutRequest AppRequest { get; set; }
}
