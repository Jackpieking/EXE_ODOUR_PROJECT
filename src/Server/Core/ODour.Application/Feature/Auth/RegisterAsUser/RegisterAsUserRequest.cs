using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Auth.RegisterAsUser;

public sealed class RegisterAsUserRequest : IFeatureRequest<RegisterAsUserResponse>
{
    public string Email { get; init; }

    public string Password { get; init; }
}
