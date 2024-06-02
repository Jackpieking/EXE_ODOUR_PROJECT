using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Auth.Logout;

public sealed class LogoutRequest : IFeatureRequest<LogoutResponse>
{
    public string RefreshToken { get; init; }
}
