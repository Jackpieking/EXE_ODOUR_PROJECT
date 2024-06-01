using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using ODour.Application.Share.Common;
using ODour.Application.Share.DataProtection;
using ODour.Application.Share.Features;
using ODour.Application.Share.Mail;
using ODour.Domain.Feature.Main;
using ODour.Domain.Share.User.Entities;

namespace ODour.Application.Feature.Auth.RegisterAsUser;

internal sealed class RegisterAsUserHandler
    : IFeatureHandler<RegisterAsUserRequest, RegisterAsUserResponse>
{
    private readonly Lazy<IMainUnitOfWork> _unitOfWork;
    private readonly Lazy<UserManager<UserEntity>> _userManager;
    private readonly Lazy<IDataProtectionHandler> _dataProtectionHandler;
    private readonly Lazy<ISendingMailHandler> _sendingMailHandler;

    public RegisterAsUserHandler(
        Lazy<IMainUnitOfWork> unitOfWork,
        Lazy<UserManager<UserEntity>> userManager,
        Lazy<IDataProtectionHandler> dataProtectionHandler,
        Lazy<ISendingMailHandler> sendingMailHandler
    )
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _dataProtectionHandler = dataProtectionHandler;
        _sendingMailHandler = sendingMailHandler;
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
    public async Task<RegisterAsUserResponse> ExecuteAsync(
        RegisterAsUserRequest command,
        CancellationToken ct
    )
    {
        #region InputValidation
        // Init new user.
        UserEntity newUser = new() { Id = Guid.NewGuid() };

        // Is new user password valid.
        var isPasswordValid = await ValidateUserPasswordAsync(
            newUser: newUser,
            newPassword: command.Password
        );

        // Password is not valid.
        if (!isPasswordValid)
        {
            return new() { StatusCode = RegisterAsUserResponseStatusCode.INPUT_VALIDATION_FAIL };
        }
        #endregion

        // Does user exist by email.
        var isUserFound =
            await _unitOfWork.Value.RegisterAsUserRepository.IsUserFoundByNormalizedEmailQueryAsync(
                email: command.Email,
                ct: ct
            );

        // User with email already exists.
        if (isUserFound)
        {
            return new() { StatusCode = RegisterAsUserResponseStatusCode.USER_IS_EXISTED };
        }

        // Get account status for new user.
        var newAccountStatus =
            await _unitOfWork.Value.RegisterAsUserRepository.GetPendingConfirmedStatusQueryAsync(
                ct: ct
            );

        // Get admin email confirmed tokens.
        var userEmailConfirmedTokens = GenerateUserEmailConfirmedToken(userId: newUser.Id);

        // Completing new user.
        FinishFillingUser(
            newUser: newUser,
            command: command,
            newAccountStatus: newAccountStatus.Id
        );

        // Create and add user to role.
        var dbResult =
            await _unitOfWork.Value.RegisterAsUserRepository.CreateAndAddUserToRoleCommandAsync(
                newUser: newUser,
                emailConfirmTokens: new List<UserTokenEntity>
                {
                    userEmailConfirmedTokens["MainToken"],
                    userEmailConfirmedTokens["AlternateToken"]
                },
                userManager: _userManager.Value,
                ct: ct
            );

        // Cannot create or add user to role.
        if (!dbResult)
        {
            return new() { StatusCode = RegisterAsUserResponseStatusCode.OPERATION_FAIL };
        }

        // Sending user confirmation mail.
        await SendingUserConfirmationMailAsync(
            newUser: newUser,
            emailConfirmedTokens: userEmailConfirmedTokens,
            command: command,
            ct: ct
        );

        return new() { StatusCode = RegisterAsUserResponseStatusCode.OPERATION_SUCCESS };
    }

    /// <summary>
    ///     Validates the password of a newly created user.
    /// </summary>
    /// <param name="newUser">
    ///     The newly created user.
    /// </param>
    /// <param name="newPassword">
    ///     The password to be validated.
    /// </param>
    /// <returns>
    ///     True if the password is valid, False otherwise.
    /// </returns>
    private async Task<bool> ValidateUserPasswordAsync(UserEntity newUser, string newPassword)
    {
        IdentityResult result = default;

        foreach (var validator in _userManager.Value.PasswordValidators)
        {
            result = await validator.ValidateAsync(
                manager: _userManager.Value,
                user: newUser,
                password: newPassword
            );
        }

        if (Equals(objA: result, objB: default))
        {
            return false;
        }

        return result.Succeeded;
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
                            plaintext: $"main{CommonConstant.App.DefaultStringSeparator}{userId}"
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
                            plaintext: $"alternate{CommonConstant.App.DefaultStringSeparator}{userId}"
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
    ///     Finishes filling the user with default
    ///     values for the newly created user.
    /// </summary>
    /// <param name="newUser">
    ///     The newly created user.
    /// </param>
    /// <param name="command">
    ///     Request model.
    /// </param>
    /// <param name="newAccountStatus">
    ///     The new account status.
    /// </param>
    /// <returns>
    ///     Nothing
    /// </returns>
    private void FinishFillingUser(
        UserEntity newUser,
        RegisterAsUserRequest command,
        Guid newAccountStatus
    )
    {
        newUser.Email = command.Email;
        newUser.UserName = command.Email;
        newUser.PasswordHash = _dataProtectionHandler.Value.Protect(
            plaintext: $"{newUser.Email.ToUpper()}{CommonConstant.App.DefaultStringSeparator}{command.Password}"
        );
        newUser.UserDetail = new()
        {
            UserId = newUser.Id,
            FirstName = string.Empty,
            LastName = string.Empty,
            AvatarUrl = CommonConstant.App.DefaultAvatarUrl,
            Gender = true,
            IsTemporarilyRemoved = false,
            AccountStatusId = newAccountStatus
        };
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
        RegisterAsUserRequest command,
        UserEntity newUser,
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
