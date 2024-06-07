using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Share.Mail;

namespace ODour.Application.Feature.Auth.Login.BackgroundJob;

internal sealed class NotifyUserAboutLoginActionByEmailEventHandler
    : IEventHandler<NotifyUserAboutLoginActionByEmailEvent>
{
    private readonly Lazy<ISendingMailHandler> _sendingMailHandler;

    public NotifyUserAboutLoginActionByEmailEventHandler(
        Lazy<ISendingMailHandler> sendingMailHandler
    )
    {
        _sendingMailHandler = sendingMailHandler;
    }

    public async Task HandleAsync(
        NotifyUserAboutLoginActionByEmailEvent eventModel,
        CancellationToken ct
    )
    {
        var mailContent =
            await _sendingMailHandler.Value.GetNotifyUserAboutLoginActionMailContentAsync(
                to: eventModel.Email,
                subject: "Cảnh báo đăng nhập",
                currentTimeInLocal: TimeZoneInfo.ConvertTimeFromUtc(
                    dateTime: eventModel.CurrentTimeInUtc,
                    destinationTimeZone: TimeZoneInfo.FindSystemTimeZoneById(
                        id: "SE Asia Standard Time"
                    )
                ),
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
