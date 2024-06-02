using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.WebUtilities;
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
        var mainToken = WebEncoders.Base64UrlEncode(
            input: Encoding.UTF8.GetBytes(s: command.MainTokenValue)
        );

        var mailContent = await _sendingMailHandler.Value.GetUserResetPasswordMailContentAsync(
            to: command.Email,
            subject: "Đổi mật khẩu",
            resetPasswordLink: $"auth/passwordChanging?token={mainToken}",
            cancellationToken: ct
        );

        // Try to send mail.
        await _sendingMailHandler.Value.SendAsync(mailContent: mailContent, cancellationToken: ct);
    }
}
