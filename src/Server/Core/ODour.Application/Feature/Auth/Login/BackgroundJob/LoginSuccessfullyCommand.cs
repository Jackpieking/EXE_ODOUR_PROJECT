using System;
using FastEndpoints;

namespace ODour.Application.Feature.Auth.Login.BackgroundJob;

internal sealed class LoginSuccessfullyCommand : ICommand
{
    public string Email { get; init; }

    public DateTime CurrentTimeInUtc { get; init; }
}
