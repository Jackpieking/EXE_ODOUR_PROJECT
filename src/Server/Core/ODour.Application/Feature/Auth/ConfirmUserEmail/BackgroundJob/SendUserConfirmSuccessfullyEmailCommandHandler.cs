using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Share.Mail;

namespace ODour.Application.Feature.Auth.ConfirmUserEmail.BackgroundJob;

internal sealed class SendUserConfirmSuccessfullyEmailCommandHandler
    : ICommandHandler<SendUserConfirmSuccessfullyEmailCommand>
{
    private readonly Lazy<ISendingMailHandler> _sendingMailHandler;

    public SendUserConfirmSuccessfullyEmailCommandHandler(
        Lazy<ISendingMailHandler> sendingMailHandler
    )
    {
        _sendingMailHandler = sendingMailHandler;
    }

    public async Task ExecuteAsync(
        SendUserConfirmSuccessfullyEmailCommand command,
        CancellationToken ct
    )
    {
        var mailContent =
            await _sendingMailHandler.Value.GetUserConfirmSuccessfullyMailContentAsync(
                to: command.Email,
                subject: "Xác nhận tài khoản thành công",
                cancellationToken: ct
            );

        // Try to send mail.
        var result = await _sendingMailHandler.Value.SendAsync(mailContent, ct);

        if (!result)
        {
            throw new ApplicationException(message: "Cannot send mail. Please try again later.");
        }
    }
}
