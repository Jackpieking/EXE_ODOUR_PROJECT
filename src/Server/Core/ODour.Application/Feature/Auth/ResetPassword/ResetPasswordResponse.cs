using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Auth.ResetPassword;

public sealed class ResetPasswordResponse : IFeatureResponse
{
    public ResetPasswordResponseStatusCode StatusCode { get; init; }
}
