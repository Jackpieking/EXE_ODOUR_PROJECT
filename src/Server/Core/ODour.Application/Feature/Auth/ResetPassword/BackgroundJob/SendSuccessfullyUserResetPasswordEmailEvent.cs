using FastEndpoints;

namespace ODour.Application.Feature.Auth.ResetPassword.BackgroundJob;

public sealed class SendSuccessfullyUserResetPasswordEmailEvent : IEvent
{
    public string Email { get; set; }
}
