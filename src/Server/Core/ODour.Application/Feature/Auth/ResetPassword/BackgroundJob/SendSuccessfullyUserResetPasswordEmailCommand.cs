using FastEndpoints;

namespace ODour.Application.Feature.Auth.ResetPassword.BackgroundJob;

public sealed class SendSuccessfullyUserResetPasswordEmailCommand : ICommand
{
    public string Email { get; set; }
}
