using FastEndpoints;

namespace ODour.Application.Feature.Auth.RegisterAsUser.BackgroundJob;

public sealed class SendingUserConfirmationEvent : IEvent
{
    public string MainTokenValue { get; init; }

    public string Email { get; init; }
}
