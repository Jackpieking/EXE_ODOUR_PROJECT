using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
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

    private readonly Lazy<ISendingMailHandler> _sendingMailHandler;

    public RegisterAsUserHandler(
        Lazy<IMainUnitOfWork> unitOfWork,
        Lazy<UserManager<UserEntity>> userManager,
        Lazy<ISendingMailHandler> sendingMailHandler
    )
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
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
                password: command.Password,
                userManager: _userManager.Value,
                ct: ct
            );

        // Cannot create or add user to role.
        if (!dbResult)
        {
            return new() { StatusCode = RegisterAsUserResponseStatusCode.OPERATION_FAIL };
        }

        // Getting mail content and sending.
        var accountConfirmedCode = WebEncoders.Base64UrlEncode(
            input: Encoding.UTF8.GetBytes(
                s: await _userManager.Value.GenerateEmailConfirmationTokenAsync(user: newUser)
            )
        );

        var mainContent =
            await _sendingMailHandler.Value.GetUserAccountConfirmationMailContentAsync(
                to: command.Email,
                subject: "Xác nhận tài khoản",
                mainVerifyLink: accountConfirmedCode,
                cancellationToken: ct
            );

        var mailSendingResult = await _sendingMailHandler.Value.SendAsync(
            mailContent: mainContent,
            cancellationToken: ct
        );

        if (!mailSendingResult)
        {
            // Delete user if sending mail fail.
            await _unitOfWork.Value.RegisterAsUserRepository.RemoveUserCommandAsync(
                newUser: newUser,
                userManager: _userManager.Value
            );

            return new() { StatusCode = RegisterAsUserResponseStatusCode.OPERATION_FAIL };
        }

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
    private static void FinishFillingUser(
        UserEntity newUser,
        RegisterAsUserRequest command,
        Guid newAccountStatus
    )
    {
        newUser.Email = command.Email;
        newUser.UserName = command.Email;
        newUser.UserDetail = new()
        {
            UserId = newUser.Id,
            FirstName = string.Empty,
            LastName = string.Empty,
            AvatarUrl = string.Empty,
            Gender = true,
            IsTemporarilyRemoved = false,
            AccountStatusId = newAccountStatus
        };
    }
}
