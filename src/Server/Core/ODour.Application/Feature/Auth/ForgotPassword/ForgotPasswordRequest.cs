using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Auth.ForgotPassword;

public sealed class ForgotPasswordRequest : IFeatureRequest<ForgotPasswordResponse>
{
    public string Email { get; set; }
}
