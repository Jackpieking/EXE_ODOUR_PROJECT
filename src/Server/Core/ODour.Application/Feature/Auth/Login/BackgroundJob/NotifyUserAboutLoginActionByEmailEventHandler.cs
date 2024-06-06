using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Share.BackgroundJob;
using ODour.Application.Share.Mail;

namespace ODour.Application.Feature.Auth.Login.BackgroundJob;

internal sealed class NotifyUserAboutLoginActionByEmailEventHandler
    : IEventHandler<NotifyUserAboutLoginActionByEmailEvent>
{
    private readonly Lazy<IJobHandler> _jobHandler;
    private readonly Lazy<ISendingMailHandler> _sendingMailHandler;

    public NotifyUserAboutLoginActionByEmailEventHandler(
        Lazy<ISendingMailHandler> sendingMailHandler,
        Lazy<IJobHandler> jobHandler
    )
    {
        _sendingMailHandler = sendingMailHandler;
        _jobHandler = jobHandler;
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
        _jobHandler.Value.ExecuteOneTimeJob(
            methodCall: () => _sendingMailHandler.Value.SendAsync(mailContent, ct)
        );
    }
}
