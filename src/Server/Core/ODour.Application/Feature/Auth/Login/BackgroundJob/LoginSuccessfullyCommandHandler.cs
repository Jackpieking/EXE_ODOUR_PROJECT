using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Share.Mail;

namespace ODour.Application.Feature.Auth.Login.BackgroundJob;

internal sealed class LoginSuccessfullyCommandHandler : ICommandHandler<LoginSuccessfullyCommand>
{
    private readonly Lazy<ISendingMailHandler> _sendingMailHandler;

    public LoginSuccessfullyCommandHandler(Lazy<ISendingMailHandler> sendingMailHandler)
    {
        _sendingMailHandler = sendingMailHandler;
    }

    public async Task ExecuteAsync(LoginSuccessfullyCommand command, CancellationToken ct)
    {
        var mailContent =
            await _sendingMailHandler.Value.GetNotifyUserAboutLoginActionMailContentAsync(
                to: command.Email,
                subject: "Cảnh báo đăng nhập",
                currentTimeInLocal: TimeZoneInfo.ConvertTimeFromUtc(
                    dateTime: command.CurrentTimeInUtc,
                    destinationTimeZone: TimeZoneInfo.FindSystemTimeZoneById(
                        id: "SE Asia Standard Time"
                    )
                ),
                cancellationToken: ct
            );

        // Try to send mail.
        var result = await _sendingMailHandler.Value.SendAsync(
            mailContent: mailContent,
            cancellationToken: ct
        );

        if (!result)
        {
            throw new ApplicationException(message: "Cannot send mail. Please try again later.");
        }
    }
}
