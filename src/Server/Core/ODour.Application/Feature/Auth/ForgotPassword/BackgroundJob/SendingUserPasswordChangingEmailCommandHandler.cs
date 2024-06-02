using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Share.Mail;

namespace ODour.Application.Feature.Auth.ForgotPassword.BackgroundJob;

internal sealed class SendingUserPasswordChangingEmailCommandHandler
    : ICommandHandler<SendingUserPasswordChangingEmailCommand>
{
    private readonly Lazy<ISendingMailHandler> _sendingMailHandler;

    public SendingUserPasswordChangingEmailCommandHandler(
        Lazy<ISendingMailHandler> sendingMailHandler
    )
    {
        _sendingMailHandler = sendingMailHandler;
    }

    public async Task ExecuteAsync(
        SendingUserPasswordChangingEmailCommand command,
        CancellationToken ct
    )
    {
        // Try to send mail.
        var result = await _sendingMailHandler.Value.SendAsync(
            mailContent: command.MailContent,
            cancellationToken: ct
        );
    }
}
