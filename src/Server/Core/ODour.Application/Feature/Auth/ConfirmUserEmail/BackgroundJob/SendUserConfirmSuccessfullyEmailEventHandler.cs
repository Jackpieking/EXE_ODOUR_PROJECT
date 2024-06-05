using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using ODour.Application.Share.BackgroundJob;
using ODour.Application.Share.Mail;

namespace ODour.Application.Feature.Auth.ConfirmUserEmail.BackgroundJob;

internal sealed class SendUserConfirmSuccessfullyEmailEventHandler
    : IEventHandler<SendUserConfirmSuccessfullyEmailEvent>
{
    private readonly Lazy<IServiceScopeFactory> _serviceScopeFactory;
    private readonly Lazy<IJobHandler> _jobHandler;

    public SendUserConfirmSuccessfullyEmailEventHandler(
        Lazy<IServiceScopeFactory> serviceScopeFactory,
        Lazy<IJobHandler> jobHandler
    )
    {
        _serviceScopeFactory = serviceScopeFactory;
        _jobHandler = jobHandler;
    }

    public async Task HandleAsync(
        SendUserConfirmSuccessfullyEmailEvent eventModel,
        CancellationToken ct
    )
    {
        await using var scope = _serviceScopeFactory.Value.CreateAsyncScope();

        var sendingMailHandler = scope.ServiceProvider.GetRequiredService<
            Lazy<ISendingMailHandler>
        >();

        var mailContent = await sendingMailHandler.Value.GetUserConfirmSuccessfullyMailContentAsync(
            to: eventModel.Email,
            subject: "Xác nhận tài khoản thành công",
            cancellationToken: ct
        );

        // Try to send mail.
        _jobHandler.Value.ExecuteOneTimeJob(
            methodCall: () => sendingMailHandler.Value.SendAsync(mailContent, ct)
        );
    }
}
