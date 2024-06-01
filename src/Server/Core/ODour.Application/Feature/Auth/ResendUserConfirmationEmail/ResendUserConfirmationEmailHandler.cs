using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.WebUtilities;
using ODour.Application.Share.Common;
using ODour.Application.Share.DataProtection;
using ODour.Application.Share.Features;
using ODour.Application.Share.Mail;
using ODour.Domain.Feature.Main;
using ODour.Domain.Share.User.Entities;

namespace ODour.Application.Feature.Auth.ResendUserConfirmationEmail;

internal sealed class ResendUserConfirmationEmailHandler
    : IFeatureHandler<ResendUserConfirmationEmailRequest, ResendUserConfirmationEmailResponse>
{
    private readonly Lazy<ISendingMailHandler> _sendingMailHandler;
    private readonly Lazy<IMainUnitOfWork> _unitOfWork;
    private readonly Lazy<IDataProtectionHandler> _dataProtectionHandler;

    public ResendUserConfirmationEmailHandler(
        Lazy<IMainUnitOfWork> unitOfWork,
        Lazy<IDataProtectionHandler> dataProtectionHandler,
        Lazy<ISendingMailHandler> sendingMailHandler
    )
    {
        _unitOfWork = unitOfWork;
        _dataProtectionHandler = dataProtectionHandler;
        _sendingMailHandler = sendingMailHandler;
    }

    public async Task<ResendUserConfirmationEmailResponse> ExecuteAsync(
        ResendUserConfirmationEmailRequest command,
        CancellationToken ct
    )
    {
        // Does user exist by email.
        var isUserFound =
            await _unitOfWork.Value.ResendUserConfirmationEmailRepository.IsUserFoundByNormalizedEmailQueryAsync(
                email: command.Email,
                ct: ct
            );

        // User with email is not found.
        if (!isUserFound)
        {
            return new()
            {
                StatusCode = ResendUserConfirmationEmailResponseStatusCode.USER_IS_NOT_FOUND
            };
        }

        // Has user confirmed email.
        var hasUserConfirmedEmail =
            await _unitOfWork.Value.ResendUserConfirmationEmailRepository.HasUserConfirmedEmailQueryAsync(
                email: command.Email,
                ct: ct
            );

        // User has confirmed email.
        if (hasUserConfirmedEmail)
        {
            return new()
            {
                StatusCode = ResendUserConfirmationEmailResponseStatusCode.USER_HAS_CONFIRMED_EMAIL
            };
        }

        // Is user temporarily removed by email.
        var IsUserTemporarilyRemoved =
            await _unitOfWork.Value.ResendUserConfirmationEmailRepository.IsUserTemporarilyRemovedQueryAsync(
                email: command.Email,
                ct: ct
            );

        // User with email is temporarily removed.
        if (IsUserTemporarilyRemoved)
        {
            return new()
            {
                StatusCode =
                    ResendUserConfirmationEmailResponseStatusCode.USER_IS_TEMPORARILY_REMOVED
            };
        }

        // Get user with user id only.
        var user =
            await _unitOfWork.Value.ResendUserConfirmationEmailRepository.GetUserByEmailQueryAsync(
                email: command.Email,
                ct: ct
            );

        // Generate email confirm token.
        var emailConfirmedTokens = GenerateUserEmailConfirmedToken(userId: user.Id);

        // Add email confirm token to the database.
        var dbResult =
            await _unitOfWork.Value.ResendUserConfirmationEmailRepository.AddUserConfirmationTokenCommandAsync(
                userTokenEntities: new List<UserTokenEntity>
                {
                    emailConfirmedTokens["MainToken"],
                    emailConfirmedTokens["AlternateToken"]
                },
                ct: ct
            );

        // Cannot add email confirm token to the database.
        if (!dbResult)
        {
            return new()
            {
                StatusCode = ResendUserConfirmationEmailResponseStatusCode.OPERATION_FAIL
            };
        }

        // Resend email again.
        await SendingUserConfirmationMailAsync(
            emailConfirmedTokens: emailConfirmedTokens,
            command: command,
            ct: ct
        );

        return new()
        {
            StatusCode = ResendUserConfirmationEmailResponseStatusCode.OPERATION_SUCCESS
        };
    }

    private Dictionary<string, UserTokenEntity> GenerateUserEmailConfirmedToken(Guid userId)
    {
        Dictionary<string, UserTokenEntity> emailConfirmedTokens = new(capacity: 2);

        // Add new token for email confirmed.
        emailConfirmedTokens.Add(
            key: "MainToken",
            value: new()
            {
                UserId = userId,
                Name = "EmailConfirmedToken",
                Value = WebEncoders.Base64UrlEncode(
                    input: Encoding.UTF8.GetBytes(
                        s: _dataProtectionHandler.Value.Protect(
                            plaintext: $"main_confirmation_email_token{CommonConstant.App.DefaultStringSeparator}{userId}"
                        )
                    )
                ),
                ExpiredAt = DateTime.UtcNow.AddHours(value: 48),
                LoginProvider = Guid.NewGuid().ToString()
            }
        );

        emailConfirmedTokens.Add(
            key: "AlternateToken",
            value: new()
            {
                UserId = userId,
                Name = "EmailConfirmedToken",
                Value = WebEncoders.Base64UrlEncode(
                    input: Encoding.UTF8.GetBytes(
                        s: _dataProtectionHandler.Value.Protect(
                            plaintext: $"alternate_confirmation_email_token{CommonConstant.App.DefaultStringSeparator}{userId}"
                        )
                    )
                ),
                ExpiredAt = DateTime.UtcNow.AddHours(value: 48),
                LoginProvider = Guid.NewGuid().ToString()
            }
        );

        return emailConfirmedTokens;
    }

    /// <summary>
    ///     Sending user confirmation mail.
    /// </summary>
    /// <param name="command">
    ///     Request model.
    /// </param>
    /// <param name="newUser">
    ///     New user.
    /// </param>
    /// <param name="ct">
    ///     The token to monitor cancellation requests.
    /// </param>
    /// <returns>
    ///     Nothing
    /// </returns>
    private async Task SendingUserConfirmationMailAsync(
        ResendUserConfirmationEmailRequest command,
        Dictionary<string, UserTokenEntity> emailConfirmedTokens,
        CancellationToken ct
    )
    {
        var mainContent =
            await _sendingMailHandler.Value.GetUserAccountConfirmationMailContentAsync(
                to: command.Email,
                subject: "Xác nhận tài khoản",
                mainLink: emailConfirmedTokens["MainToken"].Value,
                alternateLink: emailConfirmedTokens["AlternateToken"].Value,
                cancellationToken: ct
            );

        // Try to send mail.
        var sendingAnyEmailCommand = new BackgroundJob.SendingUserConfirmationCommand
        {
            MailContent = mainContent
        };

        await sendingAnyEmailCommand.QueueJobAsync(
            executeAfter: DateTime.UtcNow.AddSeconds(value: 5),
            expireOn: DateTime.UtcNow.AddMinutes(value: 5),
            ct: ct
        );
    }
}
