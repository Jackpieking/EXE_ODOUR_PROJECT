using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Share.BackgroundJob;
using ODour.Application.Share.Mail;

namespace ODour.Application.Feature.Auth.ResetPassword.BackgroundJob;

internal sealed class SendSuccessfullyUserResetPasswordEmailEventHandler
    : IEventHandler<SendSuccessfullyUserResetPasswordEmailEvent>
{
    private readonly Lazy<ISendingMailHandler> _sendingMailHandler;

    public SendSuccessfullyUserResetPasswordEmailEventHandler(
        Lazy<ISendingMailHandler> sendingMailHandler
    )
    {
        _sendingMailHandler = sendingMailHandler;
    }

    public async Task HandleAsync(
        SendSuccessfullyUserResetPasswordEmailEvent eventModel,
        CancellationToken ct
    )
    {
        var mailContent =
            await _sendingMailHandler.Value.GetUserPasswordChangedSuccessfullyMailContentAsync(
                to: eventModel.Email,
                subject: "Đổi mật khẩu thành công",
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
