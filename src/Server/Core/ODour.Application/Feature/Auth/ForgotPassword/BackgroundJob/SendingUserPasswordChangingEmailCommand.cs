using FastEndpoints;
using ODour.Application.Share.Mail;

namespace ODour.Application.Feature.Auth.ForgotPassword.BackgroundJob;

public sealed class SendingUserPasswordChangingEmailCommand : ICommand
{
    public AppMailContent MailContent { get; set; }
}
