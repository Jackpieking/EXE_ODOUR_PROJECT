using ODour.Application.Share.Features;

namespace ODour.Application.Feature.Auth.RegisterAsAdmin;

public sealed class RegisterAsAdminRequest : IFeatureRequest<RegisterAsAdminResponse>
{
    public string Email { get; init; }

    public string Password { get; init; }

    public string AdminConfirmedKey { get; init; }
}
