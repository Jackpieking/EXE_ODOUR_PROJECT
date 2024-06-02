using FastEndpoints;

namespace ODour.Application.Feature.Auth.ForgotPassword.BackgroundJob;

public sealed class SendingUserPasswordChangingEmailCommand : ICommand
{
    public string MainTokenValue { get; init; }

    public string Email { get; init; }
}
