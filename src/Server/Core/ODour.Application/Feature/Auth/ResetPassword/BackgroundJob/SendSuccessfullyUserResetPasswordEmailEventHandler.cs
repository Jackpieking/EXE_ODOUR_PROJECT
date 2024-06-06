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
    private readonly Lazy<IJobHandler> _jobHandler;

    public SendSuccessfullyUserResetPasswordEmailEventHandler(
        Lazy<ISendingMailHandler> sendingMailHandler,
        Lazy<IJobHandler> jobHandler
    )
    {
        _sendingMailHandler = sendingMailHandler;
        _jobHandler = jobHandler;
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

        // Try to send mail.
        _jobHandler.Value.ExecuteOneTimeJob(
            methodCall: () => _sendingMailHandler.Value.SendAsync(mailContent, ct)
        );
    }
}
