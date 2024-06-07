using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
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

        // Try to send mail.
        var result = await _sendingMailHandler.Value.SendAsync(mailContent, ct);

        if (!result)
        {
            for (int retryTime = 0; retryTime < 3; retryTime++)
            {
                // Try to send mail.
                result = await _sendingMailHandler.Value.SendAsync(mailContent, ct);

                if (result)
                {
                    break;
                }
            }
        }
    }
}
