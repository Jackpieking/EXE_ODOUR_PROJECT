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
        string mainLink,
        CancellationToken cancellationToken
    )
    {
        if (
            string.IsNullOrWhiteSpace(value: to)
            || string.IsNullOrWhiteSpace(value: subject)
            || string.IsNullOrWhiteSpace(value: mainLink)
        )
        {
            return default;
        }

        var mailTemplatePath = Path.Combine(
            path1: "ODOUR_USER_CONFIRMATION",
            path2: "ODOUR_USER_CONFIRMATION_EMAIL_TEMPLATE.html"
        );

        var htmlTemplate = await ReadTemplateAsync(
            templatePath: mailTemplatePath,
            cancellationToken: cancellationToken
        );

        var mailBody = new StringBuilder(value: htmlTemplate)
            .Replace(
                oldValue: "{main_link}",
                newValue: _googleGmailSendingOption.Value.WebUrl + mainLink
            )
            .Replace(
                oldValue: "{alternate_link}",
                newValue: _googleGmailSendingOption.Value.WebUrl + mainLink
            )
            .Replace(oldValue: "{app_email}", newValue: to)
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

    public async Task<AppMailContent> GetUserConfirmSuccessfullyMailContentAsync(
        string to,
        string subject,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(value: to) || string.IsNullOrWhiteSpace(value: subject))
        {
            return default;
        }

        var mailTemplatePath = Path.Combine(
            path1: "ODOUR_USER_CONFIRMATION",
            path2: "ODOUR_SUCCESSFULLY_USER_CONFIRMATION_EMAIL_TEMPLATE.html"
        );

        var htmlTemplate = await ReadTemplateAsync(
            templatePath: mailTemplatePath,
            cancellationToken: cancellationToken
        );

        return new()
        {
            To = to,
            Subject = subject,
            Body = htmlTemplate
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

    public async Task<AppMailContent> GetNotifyUserAboutLoginActionMailContentAsync(
        string to,
        string subject,
        DateTime currentTimeInLocal,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(value: to) || string.IsNullOrWhiteSpace(value: subject))
        {
            return default;
        }

        var mailTemplatePath = Path.Combine(
            path1: "ODOUR_USER_CONFIRMATION",
            path2: "NOTIFY_USER_ABOUT_LOGIN_ACTION_EMAIL_TEMPLATE.html"
        );

        var htmlTemplate = await ReadTemplateAsync(
            templatePath: mailTemplatePath,
            cancellationToken: cancellationToken
        );

        var mailBody = new StringBuilder(value: htmlTemplate)
            .Replace(oldValue: "{current-time}", newValue: currentTimeInLocal.ToString())
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

    public async Task<AppMailContent> GetUserPasswordChangedSuccessfullyMailContentAsync(
        string to,
        string subject,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(value: to) || string.IsNullOrWhiteSpace(value: subject))
        {
            return default;
        }

        var mailTemplatePath = Path.Combine(
            path1: "ODOUR_USER_CONFIRMATION",
            path2: "ODOUR_SUCCESSFULLY_USER_RESET_PASSWORD_EMAIL_TEMPLATE.html"
        );

        var htmlTemplate = await ReadTemplateAsync(
            templatePath: mailTemplatePath,
            cancellationToken: cancellationToken
        );

        return new()
        {
            To = to,
            Subject = subject,
            Body = htmlTemplate
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
        string resetPasswordLink,
        CancellationToken cancellationToken
    )
    {
        if (
            string.IsNullOrWhiteSpace(value: to)
            || string.IsNullOrWhiteSpace(value: subject)
            || string.IsNullOrWhiteSpace(value: resetPasswordLink)
        )
        {
            return default;
        }

        var mailTemplatePath = Path.Combine(
            path1: "ODOUR_USER_CONFIRMATION",
            path2: "ODOUR_USER_PASSWORD_CHANGING_EMAIL_TEMPLATE.html"
        );

        var htmlTemplate = await ReadTemplateAsync(
            templatePath: mailTemplatePath,
            cancellationToken: cancellationToken
        );

        var mailBody = new StringBuilder(value: htmlTemplate)
            .Replace(
                oldValue: "{main_link}",
                newValue: _googleGmailSendingOption.Value.WebUrl + resetPasswordLink
            )
            .Replace(
                oldValue: "{alternate_link}",
                newValue: _googleGmailSendingOption.Value.WebUrl + resetPasswordLink
            )
            .Replace(oldValue: "{app_email}", newValue: to)
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
