using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Share.Mail;

namespace ODour.Application.Feature.Auth.ResetPassword.BackgroundJob;

internal sealed class SendSuccessfullyUserResetPasswordEmailCommandHandler
    : ICommandHandler<SendSuccessfullyUserResetPasswordEmailCommand>
{
    private readonly Lazy<ISendingMailHandler> _sendingMailHandler;

    public SendSuccessfullyUserResetPasswordEmailCommandHandler(
        Lazy<ISendingMailHandler> sendingMailHandler
    )
    {
        _sendingMailHandler = sendingMailHandler;
    }

    public async Task ExecuteAsync(
        SendSuccessfullyUserResetPasswordEmailCommand command,
        CancellationToken ct
    )
    {
        var mailContent =
            await _sendingMailHandler.Value.GetUserPasswordChangedSuccessfullyMailContentAsync(
                to: command.Email,
                subject: "Đổi mật khẩu thành công",
                cancellationToken: ct
            );

        // Try to send mail.
        await _sendingMailHandler.Value.SendAsync(mailContent: mailContent, cancellationToken: ct);
    }
}
