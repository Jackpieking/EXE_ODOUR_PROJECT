using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.WebUtilities;
using ODour.Application.Share.BackgroundJob;
using ODour.Application.Share.Mail;

namespace ODour.Application.Feature.Auth.ResendUserConfirmationEmail.BackgroundJob;

internal sealed class SendingUserConfirmationEmailEventHandler
    : IEventHandler<SendingUserConfirmationEvent>
{
    private readonly Lazy<ISendingMailHandler> _sendingMailHandler;

    public SendingUserConfirmationEmailEventHandler(Lazy<ISendingMailHandler> sendingMailHandler)
    {
        _sendingMailHandler = sendingMailHandler;
    }

    public async Task HandleAsync(SendingUserConfirmationEvent eventModel, CancellationToken ct)
    {
        var mainToken = WebEncoders.Base64UrlEncode(
            input: Encoding.UTF8.GetBytes(s: eventModel.MainTokenValue)
        );

        var mailContent =
            await _sendingMailHandler.Value.GetUserAccountConfirmationMailContentAsync(
                to: eventModel.Email,
                subject: "Xác nhận tài khoản",
                mainLink: $"auth/confirmEmail?token={mainToken}",
                cancellationToken: ct
            );

        var retryTime = 3;

        do
        {
            // Try to send mail.
            var result = await _sendingMailHandler.Value.SendAsync(mailContent, ct);

            if (!result)
            {
                retryTime -= 1;
            }
        } while (retryTime != default);
    }
}
