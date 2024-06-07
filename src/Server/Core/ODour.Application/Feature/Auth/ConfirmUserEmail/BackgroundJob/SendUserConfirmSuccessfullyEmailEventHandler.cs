using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Share.BackgroundJob;
using ODour.Application.Share.Mail;

namespace ODour.Application.Feature.Auth.ConfirmUserEmail.BackgroundJob;

internal sealed class SendUserConfirmSuccessfullyEmailEventHandler
    : IEventHandler<SendUserConfirmSuccessfullyEmailEvent>
{
    private readonly Lazy<ISendingMailHandler> _sendingMailHandler;

    public SendUserConfirmSuccessfullyEmailEventHandler(
        Lazy<ISendingMailHandler> sendingMailHandler
    )
    {
        _sendingMailHandler = sendingMailHandler;
    }

    public async Task HandleAsync(
        SendUserConfirmSuccessfullyEmailEvent eventModel,
        CancellationToken ct
    )
    {
        var mailContent =
            await _sendingMailHandler.Value.GetUserConfirmSuccessfullyMailContentAsync(
                to: eventModel.Email,
                subject: "Xác nhận tài khoản thành công",
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
