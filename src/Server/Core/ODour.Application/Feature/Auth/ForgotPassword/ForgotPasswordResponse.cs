using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Auth.ForgotPassword;

public sealed class ForgotPasswordResponse : IFeatureResponse
{
    public ForgotPasswordResponseStatusCode StatusCode { get; init; }
}
