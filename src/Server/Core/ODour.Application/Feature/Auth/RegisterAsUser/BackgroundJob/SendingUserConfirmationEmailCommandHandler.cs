using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.WebUtilities;
using ODour.Application.Share.Mail;

namespace ODour.Application.Feature.Auth.RegisterAsUser.BackgroundJob;

internal sealed class SendingUserConfirmationEmailCommandHandler
    : ICommandHandler<SendingUserConfirmationCommand>
{
    private readonly Lazy<ISendingMailHandler> _sendingMailHandler;

    public SendingUserConfirmationEmailCommandHandler(Lazy<ISendingMailHandler> sendingMailHandler)
    {
        _sendingMailHandler = sendingMailHandler;
    }

    public async Task ExecuteAsync(SendingUserConfirmationCommand command, CancellationToken ct)
    {
        var mainToken = WebEncoders.Base64UrlEncode(
            input: Encoding.UTF8.GetBytes(s: command.MainTokenValue)
        );

        var mailContent =
            await _sendingMailHandler.Value.GetUserAccountConfirmationMailContentAsync(
                to: command.Email,
                subject: "Xác nhận tài khoản",
                mainLink: $"auth/confirmEmail?token={mainToken}",
                cancellationToken: ct
            );

        // Try to send mail.
        await _sendingMailHandler.Value.SendAsync(mailContent: mailContent, cancellationToken: ct);
    }
}
