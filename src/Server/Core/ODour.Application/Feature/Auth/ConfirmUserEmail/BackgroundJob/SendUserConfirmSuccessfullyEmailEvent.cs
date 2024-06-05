using FastEndpoints;

namespace ODour.Application.Feature.Auth.ConfirmUserEmail.BackgroundJob;

public sealed class SendUserConfirmSuccessfullyEmailEvent : IEvent
{
    public string Email { get; init; }
}
