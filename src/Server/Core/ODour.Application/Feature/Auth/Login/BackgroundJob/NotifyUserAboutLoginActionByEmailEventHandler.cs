using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using ODour.Application.Share.BackgroundJob;
using ODour.Application.Share.Mail;

namespace ODour.Application.Feature.Auth.Login.BackgroundJob;

internal sealed class NotifyUserAboutLoginActionByEmailEventHandler
    : IEventHandler<NotifyUserAboutLoginActionByEmailEvent>
{
    private readonly Lazy<IServiceScopeFactory> _serviceScopeFactory;
    private readonly Lazy<IJobHandler> _jobHandler;

    public NotifyUserAboutLoginActionByEmailEventHandler(
        Lazy<IServiceScopeFactory> serviceScopeFactory,
        Lazy<IJobHandler> jobHandler
    )
    {
        _serviceScopeFactory = serviceScopeFactory;
        _jobHandler = jobHandler;
    }

    public async Task HandleAsync(
        NotifyUserAboutLoginActionByEmailEvent eventModel,
        CancellationToken ct
    )
    {
        await using var scope = _serviceScopeFactory.Value.CreateAsyncScope();

        var sendingMailHandler = scope.ServiceProvider.GetRequiredService<
            Lazy<ISendingMailHandler>
        >();

        var mailContent =
            await sendingMailHandler.Value.GetNotifyUserAboutLoginActionMailContentAsync(
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
            methodCall: () => sendingMailHandler.Value.SendAsync(mailContent, ct)
        );
    }
}
