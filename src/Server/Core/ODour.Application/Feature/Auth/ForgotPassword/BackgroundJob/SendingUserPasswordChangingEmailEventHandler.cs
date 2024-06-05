using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using ODour.Application.Share.BackgroundJob;
using ODour.Application.Share.Mail;

namespace ODour.Application.Feature.Auth.ForgotPassword.BackgroundJob;

internal sealed class SendingUserPasswordChangingEmailEventHandler
    : IEventHandler<SendingUserPasswordChangingEmailEvent>
{
    private readonly Lazy<IServiceScopeFactory> _serviceScopeFactory;
    private readonly Lazy<IJobHandler> _jobHandler;

    public SendingUserPasswordChangingEmailEventHandler(
        Lazy<IServiceScopeFactory> serviceScopeFactory,
        Lazy<IJobHandler> jobHandler
    )
    {
        _serviceScopeFactory = serviceScopeFactory;
        _jobHandler = jobHandler;
    }

    public async Task HandleAsync(
        SendingUserPasswordChangingEmailEvent eventModel,
        CancellationToken ct
    )
    {
        await using var scope = _serviceScopeFactory.Value.CreateAsyncScope();

        var sendingMailHandler = scope.ServiceProvider.GetRequiredService<
            Lazy<ISendingMailHandler>
        >();

        var mainToken = WebEncoders.Base64UrlEncode(
            input: Encoding.UTF8.GetBytes(s: eventModel.MainTokenValue)
        );

        var mailContent = await sendingMailHandler.Value.GetUserResetPasswordMailContentAsync(
            to: eventModel.Email,
            subject: "Đổi mật khẩu",
            resetPasswordLink: $"auth/passwordChanging?token={mainToken}",
            cancellationToken: ct
        );

        // Try to send mail.
        _jobHandler.Value.ExecuteOneTimeJob(
            methodCall: () => sendingMailHandler.Value.SendAsync(mailContent, ct)
        );
    }
}
