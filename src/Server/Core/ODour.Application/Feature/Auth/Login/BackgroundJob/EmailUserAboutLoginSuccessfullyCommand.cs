using System;
using FastEndpoints;

namespace ODour.Application.Feature.Auth.Login.BackgroundJob;

public sealed class EmailUserAboutLoginSuccessfullyCommand : ICommand
{
    public string Email { get; init; }

    public DateTime CurrentTimeInUtc { get; init; }
}
