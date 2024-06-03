using System;
using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Auth.RefreshAccessToken;

public sealed class RefreshAccessTokenRequest : IFeatureRequest<RefreshAccessTokenResponse>
{
    public string RefreshToken { get; init; }

    // This field is assigned by system itself.
    private Guid _userId;

    public Guid GetUserId() => _userId;

    public void SetUserId(Guid userId) => _userId = userId;
}
