using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using MimeKit;
using ODour.Application.Share.Mail;
using ODour.Configuration.Infrastructure.Mail.GoogleGmail;

namespace ODour.AppNotification.Handlers;

internal sealed class GoogleSendingMailHandler : ISendingMailHandler
{
    private readonly Lazy<GoogleGmailSendingOption> _googleGmailSendingOption;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public GoogleSendingMailHandler(
        Lazy<GoogleGmailSendingOption> googleGmailSendingOption,
        IWebHostEnvironment webHostEnvironment
    )
    {
        _googleGmailSendingOption = googleGmailSendingOption;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<AppMailContent> GetUserAccountConfirmationMailContentAsync(
        string to,
        string subject,
        string verifyLink,
        CancellationToken cancellationToken
    )
    {
        if (
            string.IsNullOrWhiteSpace(value: to)
            || string.IsNullOrWhiteSpace(value: subject)
            || string.IsNullOrWhiteSpace(value: verifyLink)
        )
        {
            return default;
        }

        var mailTemplatePath = Path.Combine(
            path1: "CreateUserAccount",
            path2: "ConfirmUserAccountByEmail.html"
        );

        var htmlTemplate = await ReadTemplateAsync(
            templatePath: mailTemplatePath,
            cancellationToken: cancellationToken
        );

        var mailBody = new StringBuilder(value: htmlTemplate)
            .Replace(
                oldValue: "{verify-link}",
                newValue: _googleGmailSendingOption.Value.WebUrl + verifyLink
            )
            .ToString();

        return new()
        {
            To = to,
            Subject = subject,
            Body = mailBody
        };

        async Task<string> ReadTemplateAsync(
            string templatePath,
            CancellationToken cancellationToken
        )
        {
            var templateFilePath = Path.Combine(
                path1: _webHostEnvironment.WebRootPath,
                path2: templatePath
            );

            return await File.ReadAllTextAsync(
                path: templateFilePath,
                cancellationToken: cancellationToken
            );
        }
    }

    public async Task<AppMailContent> GetUserResetPasswordMailContentAsync(
        string to,
        string subject,
        string resetPasswordToken,
        CancellationToken cancellationToken
    )
    {
        if (
            string.IsNullOrWhiteSpace(value: to)
            || string.IsNullOrWhiteSpace(value: subject)
            || string.IsNullOrWhiteSpace(value: resetPasswordToken)
        )
        {
            return default;
        }

        var mailTemplatePath = Path.Combine(
            path1: "CreateUserAccount",
            path2: "UserResetPasswordHtmlTemplate.html"
        );

        var htmlTemplate = await ReadTemplateAsync(
            templatePath: mailTemplatePath,
            cancellationToken: cancellationToken
        );

        var mailBody = new StringBuilder(value: htmlTemplate)
            .Replace(oldValue: "{passcode}", newValue: resetPasswordToken)
            .ToString();

        return new()
        {
            To = to,
            Subject = subject,
            Body = mailBody
        };

        async Task<string> ReadTemplateAsync(
            string templatePath,
            CancellationToken cancellationToken
        )
        {
            var templateFilePath = Path.Combine(_webHostEnvironment.WebRootPath, templatePath);

            return await File.ReadAllTextAsync(templateFilePath, cancellationToken);
        }
    }

    public async Task<bool> SendAsync(
        AppMailContent mailContent,
        CancellationToken cancellationToken
    )
    {
        if (Equals(objA: mailContent, objB: default))
        {
            return false;
        }

        MimeMessage email =
            new()
            {
                Sender = new(
                    name: _googleGmailSendingOption.Value.DisplayName,
                    address: _googleGmailSendingOption.Value.Mail
                ),
                From =
                {
                    new MailboxAddress(
                        name: _googleGmailSendingOption.Value.DisplayName,
                        address: _googleGmailSendingOption.Value.Mail
                    )
                },
                To = { MailboxAddress.Parse(text: mailContent.To) },
                Subject = mailContent.Subject,
                Body = new BodyBuilder { HtmlBody = mailContent.Body }.ToMessageBody()
            };

        try
        {
            using SmtpClient smtp = new();

            await smtp.ConnectAsync(
                host: _googleGmailSendingOption.Value.Host,
                port: _googleGmailSendingOption.Value.Port,
                options: SecureSocketOptions.StartTlsWhenAvailable,
                cancellationToken: cancellationToken
            );

            await smtp.AuthenticateAsync(
                userName: _googleGmailSendingOption.Value.Mail,
                password: _googleGmailSendingOption.Value.Password,
                cancellationToken: cancellationToken
            );

            await smtp.SendAsync(message: email, cancellationToken: cancellationToken);

            await smtp.DisconnectAsync(quit: true, cancellationToken: cancellationToken);
        }
        catch
        {
            return false;
        }

        return true;
    }
}
