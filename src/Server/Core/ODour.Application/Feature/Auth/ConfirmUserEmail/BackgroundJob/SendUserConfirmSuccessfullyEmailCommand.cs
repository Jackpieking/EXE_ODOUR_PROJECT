using FastEndpoints;

namespace ODour.Application.Feature.Auth.ConfirmUserEmail.BackgroundJob;

public sealed class SendUserConfirmSuccessfullyEmailCommand : ICommand
{
    public string Email { get; init; }
}
