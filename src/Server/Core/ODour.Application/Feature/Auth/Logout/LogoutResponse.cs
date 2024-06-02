using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Auth.Logout;

public sealed class LogoutResponse : IFeatureResponse
{
    public LogoutResponseStatusCode StatusCode { get; init; }
}
