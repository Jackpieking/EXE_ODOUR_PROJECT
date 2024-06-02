using FastEndpoints;

namespace ODour.Application.Feature.Auth.RegisterAsAdmin.BackgroundJob;

public sealed class SendingUserConfirmationCommand : ICommand
{
    public string MainTokenValue { get; init; }

    public string Email { get; init; }
}
