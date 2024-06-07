using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using ODour.Application.Share.Common;
using ODour.Application.Share.DataProtection;
using ODour.Application.Share.Features;
using ODour.Domain.Feature.Main;
using ODour.Domain.Share.User.Entities;

namespace ODour.Application.Feature.Auth.RegisterAsUser;

internal sealed class RegisterAsUserHandler
    : IFeatureHandler<RegisterAsUserRequest, RegisterAsUserResponse>
{
    private readonly Lazy<IMainUnitOfWork> _unitOfWork;
    private readonly Lazy<UserManager<UserEntity>> _userManager;
    private readonly Lazy<IDataProtectionHandler> _dataProtectionHandler;

    public RegisterAsUserHandler(
        Lazy<IMainUnitOfWork> unitOfWork,
        Lazy<UserManager<UserEntity>> userManager,
        Lazy<IDataProtectionHandler> dataProtectionHandler
    )
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _dataProtectionHandler = dataProtectionHandler;
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
            await _unitOfWork.Value.RegisterAsUserRepository.GetPendingConfirmedAccountStatusQueryAsync(
                ct: ct
            );

        // Completing new user.
        FinishFillingUser(
            newUser: newUser,
            command: command,
            newAccountStatus: newAccountStatus.Id
        );

        // Create and add user to role.
        var emailConfirmedToken =
            await _unitOfWork.Value.RegisterAsUserRepository.CreateAndAddUserToRoleCommandAsync(
                newUser: newUser,
                password: command.Password,
                userManager: _userManager.Value,
                ct: ct
            );

        // Cannot create or add user to role.
        if (string.IsNullOrWhiteSpace(value: emailConfirmedToken))
        {
            return new() { StatusCode = RegisterAsUserResponseStatusCode.OPERATION_FAIL };
        }

        // Sending user confirmation mail.
        await SendingUserConfirmationMailAsync(
            emailConfirmedToken: emailConfirmedToken,
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
        newUser.UserDetail = new()
        {
            AppPasswordHash = _dataProtectionHandler.Value.Protect(
                plaintext: $"{newUser.Email.ToUpper()}{CommonConstant.App.DefaultStringSeparator}{command.Password}"
            ),
            UserId = newUser.Id,
            FirstName = string.Empty,
            LastName = string.Empty,
            AvatarUrl = CommonConstant.App.DefaultAvatarUrl,
            Gender = true,
            IsTemporarilyRemoved = false,
            AccountStatusId = newAccountStatus,
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
    private static async Task SendingUserConfirmationMailAsync(
        RegisterAsUserRequest command,
        string emailConfirmedToken,
        CancellationToken ct
    )
    {
        // Try to send mail.
        var sendingEmailEvent = new BackgroundJob.SendingUserConfirmationEvent
        {
            MainTokenValue = emailConfirmedToken,
            Email = command.Email
        };

        await sendingEmailEvent.PublishAsync(waitMode: Mode.WaitForNone, cancellation: ct);
    }
}
