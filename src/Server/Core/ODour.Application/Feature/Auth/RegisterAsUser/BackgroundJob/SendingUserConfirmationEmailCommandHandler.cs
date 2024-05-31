using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Share.Mail;

namespace ODour.Application.Feature.Auth.RegisterAsUser.BackgroundJob;

internal sealed class SendingUserConfirmationEmailCommandHandler
    : ICommandHandler<SendingUserConfirmationCommand>
{
    private readonly Lazy<ISendingMailHandler> _sendingMailHandler;

    public SendingUserConfirmationEmailCommandHandler(Lazy<ISendingMailHandler> sendingMailHandler)
    {
        _sendingMailHandler = sendingMailHandler;
    }

    public async Task ExecuteAsync(SendingUserConfirmationCommand command, CancellationToken ct)
    {
        // Try to send mail.
        var result = await _sendingMailHandler.Value.SendAsync(
            mailContent: command.MailContent,
            cancellationToken: ct
        );
    }
}
