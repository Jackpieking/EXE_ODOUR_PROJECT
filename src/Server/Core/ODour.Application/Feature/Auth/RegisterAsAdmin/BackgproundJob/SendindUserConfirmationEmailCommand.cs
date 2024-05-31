using FastEndpoints;
using ODour.Application.Share.Mail;

namespace ODour.Application.Feature.Auth.RegisterAsAdmin.BackgroundJob;

public sealed class SendingUserConfirmationCommand : ICommand
{
    public AppMailContent MailContent { get; set; }
}
