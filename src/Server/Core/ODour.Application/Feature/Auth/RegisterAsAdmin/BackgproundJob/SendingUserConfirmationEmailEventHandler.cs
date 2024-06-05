using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using ODour.Application.Share.BackgroundJob;
using ODour.Application.Share.Mail;

namespace ODour.Application.Feature.Auth.RegisterAsAdmin.BackgroundJob;

internal sealed class SendingUserConfirmationEmailEventHandler
    : IEventHandler<SendingUserConfirmationEvent>
{
    private readonly Lazy<IServiceScopeFactory> _serviceScopeFactory;
    private readonly Lazy<IJobHandler> _jobHandler;

    public SendingUserConfirmationEmailEventHandler(
        Lazy<IServiceScopeFactory> serviceScopeFactory,
        Lazy<IJobHandler> jobHandler
    )
    {
        _serviceScopeFactory = serviceScopeFactory;
        _jobHandler = jobHandler;
    }

    public async Task HandleAsync(SendingUserConfirmationEvent eventModel, CancellationToken ct)
    {
        await using var scope = _serviceScopeFactory.Value.CreateAsyncScope();

        var sendingMailHandler = scope.ServiceProvider.GetRequiredService<
            Lazy<ISendingMailHandler>
        >();

        var mainToken = WebEncoders.Base64UrlEncode(
            input: Encoding.UTF8.GetBytes(s: eventModel.MainTokenValue)
        );

        var mailContent = await sendingMailHandler.Value.GetUserAccountConfirmationMailContentAsync(
            to: eventModel.Email,
            subject: "Xác nhận tài khoản",
            mainLink: $"admin/auth/confirmEmail?token={mainToken}",
            cancellationToken: ct
        );

        // Try to send mail.
        _jobHandler.Value.ExecuteOneTimeJob(
            methodCall: () => sendingMailHandler.Value.SendAsync(mailContent, ct)
        );
    }
}
