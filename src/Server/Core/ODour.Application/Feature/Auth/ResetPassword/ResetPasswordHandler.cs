using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
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

    public ResetPasswordHandler(
        Lazy<IMainUnitOfWork> unitOfWork,
        Lazy<UserManager<UserEntity>> userManager
    )
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<ResetPasswordResponse> ExecuteAsync(
        ResetPasswordRequest command,
        CancellationToken ct
    )
    {
        #region InputValidation
        // Validate input token.
        var (userId, tokenId, tokenValue) = await ValidateTokenAsync(
            token: command.ResetPasswordToken,
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
            await _unitOfWork.Value.ResetPasswordRepository.IsUserTemporarilyRemovedQueryAsync(
                userId: userId,
                ct: ct
            );

        // User with id is temporarily removed.
        if (IsUserTemporarilyRemoved)
        {
            return new()
            {
                StatusCode = ResetPasswordResponseStatusCode.USER_IS_TEMPORARILY_REMOVED
            };
        }

        var foundUser = await _userManager.Value.FindByIdAsync(userId: userId.ToString());

        // Reset user password.
        var dbResult = await _unitOfWork.Value.ResetPasswordRepository.ResetPasswordCommandAsync(
            user: foundUser,
            tokenId: tokenId,
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

    private async Task<(Guid, Guid, string)> ValidateTokenAsync(string token, CancellationToken ct)
    {
        token = Encoding.UTF8.GetString(bytes: WebEncoders.Base64UrlDecode(input: token));

        if (string.IsNullOrWhiteSpace(value: token))
        {
            return (Guid.Empty, Guid.Empty, string.Empty);
        }

        var tokens = token.Split(separator: CommonConstant.App.DefaultStringSeparator);

        if (!tokens.Any())
        {
            return (Guid.Empty, Guid.Empty, string.Empty);
        }

        var foundUserToken =
            await _unitOfWork.Value.ResetPasswordRepository.GetResetPasswordTokenByTokenIdQueryAsync(
                tokenId: tokens[1],
                ct: ct
            );

        if (Equals(objA: foundUserToken, objB: default))
        {
            return (Guid.Empty, Guid.Empty, string.Empty);
        }

        return (foundUserToken.UserId, Guid.Parse(input: tokens[1]), tokens[0]);
    }

    private static async Task SendingUserResetPasswordSuccessfullyMailAsync(
        string email,
        CancellationToken ct
    )
    {
        // Try to send mail.
        var sendingAnyEmailCommand = new BackgroundJob.SendSuccessfullyUserResetPasswordEmailCommand
        {
            Email = email
        };

        await sendingAnyEmailCommand.QueueJobAsync(
            expireOn: DateTime.UtcNow.AddMinutes(value: 5),
            ct: ct
        );
    }
}
