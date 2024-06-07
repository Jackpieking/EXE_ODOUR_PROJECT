using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using ODour.Application.Share.BackgroundJob;
using ODour.Application.Share.Common;
using ODour.Application.Share.Features;
using ODour.Domain.Feature.Main;
using ODour.Domain.Share.User.Entities;

namespace ODour.Application.Feature.Auth.ResetPassword;

public sealed class ResetPasswordHandler
    : IFeatureHandler<ResetPasswordRequest, ResetPasswordResponse>
{
    private readonly Lazy<IMainUnitOfWork> _unitOfWork;
    private readonly Lazy<UserManager<UserEntity>> _userManager;
    private readonly Lazy<IQueueHandler> _queueHandler;

    public ResetPasswordHandler(
        Lazy<IMainUnitOfWork> unitOfWork,
        Lazy<UserManager<UserEntity>> userManager,
        Lazy<IQueueHandler> queueHandler
    )
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _queueHandler = queueHandler;
    }

    public async Task<ResetPasswordResponse> ExecuteAsync(
        ResetPasswordRequest command,
        CancellationToken ct
    )
    {
        #region InputValidation
        // Validate input token.
        var (userId, tokenValue) = await ValidateTokenAsync(
            tokenValue: command.ResetPasswordToken,
            ct: ct
        );

        // Invalid token.
        if (userId == Guid.Empty)
        {
            return new() { StatusCode = ResetPasswordResponseStatusCode.INPUT_VALIDATION_FAIL };
        }

        // Is new user password valid.
        var isPasswordValid = await ValidateUserPasswordAsync(
            user: new() { Id = userId },
            newPassword: command.NewPassword
        );

        // Password is not valid.
        if (!isPasswordValid)
        {
            return new() { StatusCode = ResetPasswordResponseStatusCode.INPUT_VALIDATION_FAIL };
        }
        #endregion

        // Does user exist by user id.
        var isUserFound =
            await _unitOfWork.Value.ResetPasswordRepository.IsUserFoundByUserIdQueryAsync(
                userId: userId,
                ct: ct
            );

        // User with id is not found.
        if (!isUserFound)
        {
            return new() { StatusCode = ResetPasswordResponseStatusCode.USER_IS_NOT_FOUND };
        }

        // Is user temporarily removed by id.
        var IsUserTemporarilyRemoved =
            await _unitOfWork.Value.ResetPasswordRepository.IsUserBannedQueryAsync(
                userId: userId,
                ct: ct
            );

        // User with id is temporarily removed.
        if (IsUserTemporarilyRemoved)
        {
            return new()
            {
                StatusCode = ResetPasswordResponseStatusCode.USER_IS_TEMPORARILY_BANNED
            };
        }

        var foundUser = await _userManager.Value.FindByIdAsync(userId: userId.ToString());

        // Reset user password.
        var dbResult = await _unitOfWork.Value.ResetPasswordRepository.ResetPasswordCommandAsync(
            user: foundUser,
            tokenValue: tokenValue,
            newPassword: command.NewPassword,
            userManager: _userManager.Value,
            ct: ct
        );

        // Cannot reset user password.
        if (!dbResult)
        {
            return new() { StatusCode = ResetPasswordResponseStatusCode.OPERATION_FAIL };
        }

        await SendingUserResetPasswordSuccessfullyMailAsync(email: foundUser.Email, ct: ct);

        return new() { StatusCode = ResetPasswordResponseStatusCode.OPERATION_SUCCESS };
    }

    /// <summary>
    ///     Validates the password of a newly created user.
    /// </summary>
    /// <param name="user">
    ///     user.
    /// </param>
    /// <param name="newPassword">
    ///     The password to be validated.
    /// </param>
    /// <returns>
    ///     True if the password is valid, False otherwise.
    /// </returns>
    private async Task<bool> ValidateUserPasswordAsync(UserEntity user, string newPassword)
    {
        IdentityResult result = default;

        foreach (var validator in _userManager.Value.PasswordValidators)
        {
            result = await validator.ValidateAsync(
                manager: _userManager.Value,
                user: user,
                password: newPassword
            );
        }

        if (Equals(objA: result, objB: default))
        {
            return false;
        }

        return result.Succeeded;
    }

    private async Task<(Guid, string)> ValidateTokenAsync(string tokenValue, CancellationToken ct)
    {
        tokenValue = Encoding.UTF8.GetString(bytes: WebEncoders.Base64UrlDecode(input: tokenValue));

        if (string.IsNullOrWhiteSpace(value: tokenValue))
        {
            return (Guid.Empty, string.Empty);
        }

        var foundUserToken =
            await _unitOfWork.Value.ResetPasswordRepository.GetResetPasswordTokenByTokenIdQueryAsync(
                tokenValue: tokenValue,
                ct: ct
            );

        if (Equals(objA: foundUserToken, objB: default))
        {
            return (Guid.Empty, string.Empty);
        }

        return (foundUserToken.UserId, tokenValue);
    }

    private async Task SendingUserResetPasswordSuccessfullyMailAsync(
        string email,
        CancellationToken ct
    )
    {
        // Try to send mail.
        var sendingEmailCommand = new BackgroundJob.SendSuccessfullyUserResetPasswordEmailCommand
        {
            Email = email
        };

        await _queueHandler.Value.QueueAsync(sendingEmailCommand, ct: ct);
    }
}
