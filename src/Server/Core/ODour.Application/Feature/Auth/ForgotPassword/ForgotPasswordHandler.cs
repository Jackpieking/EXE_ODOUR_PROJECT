using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using ODour.Application.Share.Common;
using ODour.Application.Share.Features;
using ODour.Domain.Feature.Main;
using ODour.Domain.Share.User.Entities;

namespace ODour.Application.Feature.Auth.ForgotPassword;

internal sealed class ForgotPasswordHandler
    : IFeatureHandler<ForgotPasswordRequest, ForgotPasswordResponse>
{
    private readonly Lazy<IMainUnitOfWork> _unitOfWork;
    private readonly Lazy<UserManager<UserEntity>> _userManager;

    public ForgotPasswordHandler(
        Lazy<IMainUnitOfWork> unitOfWork,
        Lazy<UserManager<UserEntity>> userManager
    )
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<ForgotPasswordResponse> ExecuteAsync(
        ForgotPasswordRequest command,
        CancellationToken ct
    )
    {
        // Does user exist by email.
        var isUserFound =
            await _unitOfWork.Value.ForgotPasswordRepository.IsUserFoundByNormalizedEmailQueryAsync(
                email: command.Email,
                ct: ct
            );

        // User with email is not found.
        if (!isUserFound)
        {
            return new() { StatusCode = ForgotPasswordResponseStatusCode.USER_IS_NOT_FOUND };
        }

        // Is user temporarily removed by email.
        var IsUserTemporarilyRemoved =
            await _unitOfWork.Value.ForgotPasswordRepository.IsUserTemporarilyRemovedQueryAsync(
                email: command.Email,
                ct: ct
            );

        // User with email is temporarily removed.
        if (IsUserTemporarilyRemoved)
        {
            return new()
            {
                StatusCode = ForgotPasswordResponseStatusCode.USER_IS_TEMPORARILY_BANNED
            };
        }

        // Get user with user id only.
        var user = await _userManager.Value.FindByEmailAsync(email: command.Email);

        // Generate password changing token.
        var passwordChangingTokens = await GenerateUserPasswordChangingTokenAsync(user: user);

        // Add token to the database.
        var dbResult =
            await _unitOfWork.Value.ResendUserConfirmationEmailRepository.AddUserPasswordChangingTokenCommandAsync(
                userTokenEntities: new List<UserTokenEntity>
                {
                    passwordChangingTokens["MainToken"]
                },
                ct: ct
            );

        // Cannot add token to the database.
        if (!dbResult)
        {
            return new() { StatusCode = ForgotPasswordResponseStatusCode.OPERATION_FAIL };
        }

        // Send email.
        await SendingUserPasswordChangingMailAsync(
            passwordChangingTokens: passwordChangingTokens,
            command: command,
            ct: ct
        );

        return new() { StatusCode = ForgotPasswordResponseStatusCode.OPERATION_SUCCESS };
    }

    private async Task<Dictionary<string, UserTokenEntity>> GenerateUserPasswordChangingTokenAsync(
        UserEntity user
    )
    {
        Dictionary<string, UserTokenEntity> emailConfirmedTokens = new(capacity: 2);

        var tokenId = Guid.NewGuid();

        // Add new token for password changing.
        emailConfirmedTokens.Add(
            key: "MainToken",
            value: new()
            {
                UserId = user.Id,
                Name = "PasswordChanghingToken",
                Value =
                    $"{await _userManager.Value.GeneratePasswordResetTokenAsync(user: user)}{CommonConstant.App.DefaultStringSeparator}{tokenId}",
                ExpiredAt = DateTime.UtcNow.AddMinutes(value: 1),
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
    /// <param name="newUser">
    ///     New user.
    /// </param>
    /// <param name="ct">
    ///     The token to monitor cancellation requests.
    /// </param>
    /// <returns>
    ///     Nothing
    /// </returns>
    private static async Task SendingUserPasswordChangingMailAsync(
        ForgotPasswordRequest command,
        Dictionary<string, UserTokenEntity> passwordChangingTokens,
        CancellationToken ct
    )
    {
        //Try to send mail.
        var sendingAnyEmailCommand = new BackgroundJob.SendingUserPasswordChangingEmailCommand
        {
            MainTokenValue = passwordChangingTokens["MainToken"].Value,
            Email = command.Email
        };

        await sendingAnyEmailCommand.QueueJobAsync(
            expireOn: DateTime.UtcNow.AddMinutes(value: 5),
            ct: ct
        );
    }
}
