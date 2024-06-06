using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.WebUtilities;
using ODour.Application.Share.BackgroundJob;
using ODour.Application.Share.Mail;

namespace ODour.Application.Feature.Auth.RegisterAsAdmin.BackgroundJob;

internal sealed class SendingUserConfirmationEmailEventHandler
    : IEventHandler<SendingUserConfirmationEvent>
{
    private readonly Lazy<ISendingMailHandler> _sendingMailHandler;
    private readonly Lazy<IJobHandler> _jobHandler;

    public SendingUserConfirmationEmailEventHandler(
        Lazy<ISendingMailHandler> sendingMailHandler,
        Lazy<IJobHandler> jobHandler
    )
    {
        _sendingMailHandler = sendingMailHandler;
        _jobHandler = jobHandler;
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
                mainLink: $"admin/auth/confirmEmail?token={mainToken}",
                cancellationToken: ct
            );

        // Try to send mail.
        _jobHandler.Value.ExecuteOneTimeJob(
            methodCall: () => _sendingMailHandler.Value.SendAsync(mailContent, ct)
        );
    }
}
