using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Auth.ResetPassword;

public sealed class ResetPasswordRequest : IFeatureRequest<ResetPasswordResponse>
{
    public string NewPassword { get; init; }

    public string ResetPasswordToken { get; init; }
}
