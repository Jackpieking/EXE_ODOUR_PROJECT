using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Auth.RefreshAccessToken;

public sealed class RefreshAccessTokenRequest : IFeatureRequest<RefreshAccessTokenResponse>
{
    public string RefreshToken { get; init; }
}
