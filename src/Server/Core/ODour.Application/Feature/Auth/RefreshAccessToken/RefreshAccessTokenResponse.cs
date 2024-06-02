using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Auth.RefreshAccessToken;

public sealed class RefreshAccessTokenResponse : IFeatureResponse
{
    public RefreshAccessTokenResponseStatusCode StatusCode { get; init; }

    public ResponseBody Body { get; init; }

    public sealed class ResponseBody
    {
        public string NewAccessToken { get; init; }
    }
}
