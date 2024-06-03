using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Auth.Logout;

public sealed class LogoutRequest : IFeatureRequest<LogoutResponse>
{
    private string _refreshToken;

    public string GetRefreshToken() => _refreshToken;

    public void SetRefreshToken(string refreshToken) => _refreshToken = refreshToken;
}
