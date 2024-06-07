using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ODour.Application.Share.BackgroundJob;
using ODour.Application.Share.Features;
using ODour.Domain.Feature.Main;
using ODour.Domain.Share.User.Entities;

namespace ODour.Application.Feature.Auth.ForgotPassword;

internal sealed class ForgotPasswordHandler
    : IFeatureHandler<ForgotPasswordRequest, ForgotPasswordResponse>
{
    private readonly Lazy<IMainUnitOfWork> _unitOfWork;
    private readonly Lazy<UserManager<UserEntity>> _userManager;
    private readonly Lazy<IQueueHandler> _queueHandler;

    public ForgotPasswordHandler(
        Lazy<IMainUnitOfWork> unitOfWork,
        Lazy<UserManager<UserEntity>> userManager,
        Lazy<IQueueHandler> queueHandler
    )
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _queueHandler = queueHandler;
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
            await _unitOfWork.Value.ForgotPasswordRepository.IsUserBannedQueryAsync(
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
        var passwordChangingToken = await GenerateUserPasswordChangingTokenAsync(user: user);

        // Add token to the database.
        var dbResult =
            await _unitOfWork.Value.ForgotPasswordRepository.AddUserPasswordChangingTokenCommandAsync(
                userTokenEntity: passwordChangingToken,
                ct: ct
            );

        // Cannot add token to the database.
        if (!dbResult)
        {
            return new() { StatusCode = ForgotPasswordResponseStatusCode.OPERATION_FAIL };
        }

        // Send email.
        await SendingUserPasswordChangingMailAsync(
            passwordChangingTokenValue: passwordChangingToken.Value,
            command: command,
            ct: ct
        );

        return new() { StatusCode = ForgotPasswordResponseStatusCode.OPERATION_SUCCESS };
    }

    private async Task<UserTokenEntity> GenerateUserPasswordChangingTokenAsync(UserEntity user)
    {
        var tokenId = Guid.NewGuid();

        return new()
        {
            UserId = user.Id,
            Name = "PasswordChanghingToken",
            Value = await _userManager.Value.GeneratePasswordResetTokenAsync(user: user),
            ExpiredAt = DateTime.UtcNow.AddMinutes(value: 1),
            LoginProvider = tokenId.ToString()
        };
    }

    /// <summary>
    ///     Sending user confirmation mail.
    /// </summary>
    /// <param name="command">
    ///     Request model.
    /// </param>
    /// <param name="passwordChangingTokenValue">
    ///     Password changing token value.
    /// </param>
    /// <param name="ct">
    ///     The token to monitor cancellation requests.
    /// </param>
    /// <returns>
    ///     Nothing
    /// </returns>
    private async Task SendingUserPasswordChangingMailAsync(
        ForgotPasswordRequest command,
        string passwordChangingTokenValue,
        CancellationToken ct
    )
    {
        //Try to send mail.
        var sendingEmailCommand = new BackgroundJob.SendingUserPasswordChangingEmailCommand
        {
            MainTokenValue = passwordChangingTokenValue,
            Email = command.Email
        };

        await _queueHandler.Value.QueueAsync(backgroundJobCommand: sendingEmailCommand, ct: ct);
    }
}
