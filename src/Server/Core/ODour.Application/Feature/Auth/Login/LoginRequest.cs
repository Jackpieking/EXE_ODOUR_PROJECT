using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Auth.Login;

public sealed class LoginRequest : IFeatureRequest<LoginResponse>
{
    public string Email { get; init; }

    public string Password { get; init; }

    public bool IsRememberMe { get; init; }
}
