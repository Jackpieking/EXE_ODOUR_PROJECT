using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.WebUtilities;
using ODour.Application.Share.Common;
using ODour.Application.Share.DataProtection;
using ODour.Application.Share.Features;
using ODour.Application.Share.Mail;
using ODour.Application.Share.Tokens;
using ODour.Domain.Feature.Main;
using ODour.Domain.Share.System.Entities;

namespace ODour.Application.Feature.Auth.RegisterAsAdmin;

internal sealed class RegisterAsAdminHandler
    : IFeatureHandler<RegisterAsAdminRequest, RegisterAsAdminResponse>
{
    private readonly Lazy<IMainUnitOfWork> _unitOfWork;
    private readonly Lazy<ISendingMailHandler> _sendingMailHandler;
    private readonly Lazy<IDataProtectionHandler> _dataProtectionHandler;
    private readonly Lazy<IAdminAccessKeyHandler> _adminAccessKeyHandler;

    public RegisterAsAdminHandler(
        Lazy<IMainUnitOfWork> unitOfWork,
        Lazy<ISendingMailHandler> sendingMailHandler,
        Lazy<IDataProtectionHandler> dataProtectionHandler,
        Lazy<IAdminAccessKeyHandler> adminAccessKeyHandler
    )
    {
        _unitOfWork = unitOfWork;
        _sendingMailHandler = sendingMailHandler;
        _dataProtectionHandler = dataProtectionHandler;
        _adminAccessKeyHandler = adminAccessKeyHandler;
    }

    /// <summary>
    ///     Entry of new request handler.
    /// </summary>
    /// <param name="command">
    ///     Request model.
    /// </param>
    /// <param name="ct">
    ///     A token that is used for notifying system
    ///     to cancel the current operation when user stop
    ///     the request.
    /// </param>
    /// <returns>
    ///     A task containing the response.
    /// </returns>
    public async Task<RegisterAsAdminResponse> ExecuteAsync(
        RegisterAsAdminRequest command,
        CancellationToken ct
    )
    {
        #region Validation
        // If user is not granted with admin access key.
        if (!command.AdminConfirmedKey.Equals(value: _adminAccessKeyHandler.Value.Get()))
        {
            return new() { StatusCode = RegisterAsAdminResponseStatusCode.FORBIDDEN };
        }
        #endregion

        // Does user exist by email.
        var isUserFound =
            await _unitOfWork.Value.RegisterAsAdminRepository.IsUserFoundByNormalizedEmailQueryAsync(
                email: command.Email,
                ct: ct
            );

        // User with email already exists.
        if (isUserFound)
        {
            return new() { StatusCode = RegisterAsAdminResponseStatusCode.USER_IS_EXISTED };
        }

        // Get account status for new user.
        var newAccountStatus =
            await _unitOfWork.Value.RegisterAsAdminRepository.GetPendingConfirmedStatusQueryAsync(
                ct: ct
            );

        // Get admin role include id for new user.
        var adminRole = await _unitOfWork.Value.RegisterAsAdminRepository.GetAdminRoleQueryAsync(
            ct: ct
        );

        // Init new admin id.
        var newAdminId = Guid.NewGuid();

        // Get admin email confirmed tokens.
        var adminEmailConfirmedTokens = GenerateAdminEmailConfirmedToken(adminId: newAdminId);

        // Add admin to database.
        var dbResult = await _unitOfWork.Value.RegisterAsAdminRepository.CreateAdminCommandAsync(
            newAdmin: InitAdmin(
                command: command,
                adminId: newAdminId,
                newUserAccountStatusId: newAccountStatus.Id
            ),
            adminRoles: new List<SystemAccountRoleEntity>
            {
                new() { RoleId = adminRole.Id, SystemAccountId = newAdminId }
            },
            adminTokens: new List<SystemAccountTokenEntity>
            {
                adminEmailConfirmedTokens["MainToken"],
                adminEmailConfirmedTokens["AlternateToken"]
            },
            ct: ct
        );

        // Cannot add.
        if (!dbResult)
        {
            return new() { StatusCode = RegisterAsAdminResponseStatusCode.OPERATION_FAIL };
        }

        // Sending add confirmation mail.
        await SendingUserConfirmationMailAsync(
            command: command,
            emailConfirmedTokens: adminEmailConfirmedTokens,
            ct: ct
        );

        return new() { StatusCode = RegisterAsAdminResponseStatusCode.OPERATION_SUCCESS };
    }

    private SystemAccountEntity InitAdmin(
        RegisterAsAdminRequest command,
        Guid newUserAccountStatusId,
        Guid adminId
    )
    {
        var upperCaseEmail = command.Email.ToUpper();

        return new()
        {
            Id = adminId,
            UserName = command.Email,
            NormalizedUserName = upperCaseEmail,
            Email = command.Email,
            NormalizedEmail = upperCaseEmail,
            EmailConfirmed = false,
            PasswordHash = _dataProtectionHandler.Value.Protect(
                plaintext: $"{upperCaseEmail}{CommonConstant.App.DefaultStringSeparator}{command.Password}"
            ),
            AccessFailedCount = default,
            LockoutEnd = CommonConstant.App.MinTimeInUTC,
            IsTemporarilyRemoved = false,
            AccountStatusId = newUserAccountStatusId
        };
    }

    private Dictionary<string, SystemAccountTokenEntity> GenerateAdminEmailConfirmedToken(
        Guid adminId
    )
    {
        Dictionary<string, SystemAccountTokenEntity> emailConfirmedTokens = new(capacity: 2);

        var tokenId = Guid.NewGuid();

        // Add new token for email confirmed.
        emailConfirmedTokens.Add(
            key: "MainToken",
            value: new()
            {
                SystemAccountId = adminId,
                Name = "EmailConfirmedToken",
                Value = _dataProtectionHandler.Value.Protect(
                    plaintext: $"main_confirmation_email_token{CommonConstant.App.DefaultStringSeparator}{tokenId}"
                ),
                ExpiredAt = DateTime.UtcNow.AddHours(value: 48),
                LoginProvider = tokenId.ToString()
            }
        );

        tokenId = Guid.NewGuid();

        emailConfirmedTokens.Add(
            key: "AlternateToken",
            value: new()
            {
                SystemAccountId = adminId,
                Name = "EmailConfirmedToken",
                Value = _dataProtectionHandler.Value.Protect(
                    plaintext: $"alternate_confirmation_email_token{CommonConstant.App.DefaultStringSeparator}{tokenId}"
                ),
                ExpiredAt = DateTime.UtcNow.AddHours(value: 48),
                LoginProvider = tokenId.ToString()
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
    /// <param name="emailConfirmedTokens">
    ///     Email confirmed tokens.
    /// </param>
    /// <param name="ct">
    ///     The token to monitor cancellation requests.
    /// </param>
    /// <returns>
    ///     Nothing
    /// </returns>
    private async Task SendingUserConfirmationMailAsync(
        RegisterAsAdminRequest command,
        Dictionary<string, SystemAccountTokenEntity> emailConfirmedTokens,
        CancellationToken ct
    )
    {
        var mainToken = WebEncoders.Base64UrlEncode(
            input: Encoding.UTF8.GetBytes(s: emailConfirmedTokens["MainToken"].Value)
        );

        var alternateToken = WebEncoders.Base64UrlEncode(
            input: Encoding.UTF8.GetBytes(s: emailConfirmedTokens["AlternateToken"].Value)
        );

        var mainContent =
            await _sendingMailHandler.Value.GetUserAccountConfirmationMailContentAsync(
                to: command.Email,
                subject: "Xác nhận tài khoản",
                mainLink: $"auth/confirmEmail?token={mainToken}",
                alternateLink: $"auth/confirmEmail?token={alternateToken}",
                cancellationToken: ct
            );
        // Try to send mail.
        var sendingAnyEmailCommand = new BackgroundJob.SendingUserConfirmationCommand
        {
            MailContent = mainContent
        };

        await sendingAnyEmailCommand.QueueJobAsync(
            expireOn: DateTime.UtcNow.AddMinutes(value: 5),
            ct: ct
        );
    }
}
