using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.WebUtilities;
using ODour.Application.Share.BackgroundJob;
using ODour.Application.Share.Mail;

namespace ODour.Application.Feature.Auth.ForgotPassword.BackgroundJob;

internal sealed class SendingUserPasswordChangingEmailEventHandler
    : IEventHandler<SendingUserPasswordChangingEmailEvent>
{
    private readonly Lazy<ISendingMailHandler> _sendingMailHandler;

    public SendingUserPasswordChangingEmailEventHandler(
        Lazy<ISendingMailHandler> sendingMailHandler
    )
    {
        _sendingMailHandler = sendingMailHandler;
    }

    public async Task HandleAsync(
        SendingUserPasswordChangingEmailEvent eventModel,
        CancellationToken ct
    )
    {
        var mainToken = WebEncoders.Base64UrlEncode(
            input: Encoding.UTF8.GetBytes(s: eventModel.MainTokenValue)
        );

        var mailContent = await _sendingMailHandler.Value.GetUserResetPasswordMailContentAsync(
            to: eventModel.Email,
            subject: "Đổi mật khẩu",
            resetPasswordLink: $"auth/passwordChanging?token={mainToken}",
            cancellationToken: ct
        );

        var retryTime = 3;

        do
        {
            // Try to send mail.
            var result = await _sendingMailHandler.Value.SendAsync(mailContent, ct);

            if (!result)
            {
                retryTime--;
            }
        } while (retryTime != 0);
    }
}
