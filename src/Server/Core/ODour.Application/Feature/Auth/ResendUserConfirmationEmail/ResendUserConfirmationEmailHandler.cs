using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ODour.Application.Share.BackgroundJob;
using ODour.Application.Share.Features;
using ODour.Domain.Feature.Main;
using ODour.Domain.Share.User.Entities;

namespace ODour.Application.Feature.Auth.ResendUserConfirmationEmail;

internal sealed class ResendUserConfirmationEmailHandler
    : IFeatureHandler<ResendUserConfirmationEmailRequest, ResendUserConfirmationEmailResponse>
{
    private readonly Lazy<IMainUnitOfWork> _unitOfWork;
    private readonly Lazy<UserManager<UserEntity>> _userManager;
    private readonly Lazy<IQueueHandler> _queueHandler;

    public ResendUserConfirmationEmailHandler(
        Lazy<IMainUnitOfWork> unitOfWork,
        Lazy<UserManager<UserEntity>> userManager,
        Lazy<IQueueHandler> queueHandler
    )
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _queueHandler = queueHandler;
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
            await _unitOfWork.Value.ResendUserConfirmationEmailRepository.IsUserBannedQueryAsync(
                email: command.Email,
                ct: ct
            );

        // User with email is temporarily removed.
        if (IsUserTemporarilyRemoved)
        {
            return new()
            {
                StatusCode =
                    ResendUserConfirmationEmailResponseStatusCode.USER_IS_TEMPORARILY_BANNED
            };
        }

        // Get user with user id only.
        var user = await _userManager.Value.FindByEmailAsync(email: command.Email);

        // Generate email confirm token.
        var emailConfirmedToken = await GenerateUserEmailConfirmedTokenAsync(user: user);

        // Add email confirm token to the database.
        var dbResult =
            await _unitOfWork.Value.ResendUserConfirmationEmailRepository.AddUserConfirmedEmailTokenCommandAsync(
                userTokenEntity: emailConfirmedToken,
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
            emailConfirmedTokenValue: emailConfirmedToken.Value,
            command: command,
            ct: ct
        );

        return new()
        {
            StatusCode = ResendUserConfirmationEmailResponseStatusCode.OPERATION_SUCCESS
        };
    }

    private async Task<UserTokenEntity> GenerateUserEmailConfirmedTokenAsync(UserEntity user)
    {
        var tokenId = Guid.NewGuid();

        return new()
        {
            UserId = user.Id,
            Name = "EmailConfirmedToken",
            Value = await _userManager.Value.GenerateEmailConfirmationTokenAsync(user: user),
            ExpiredAt = DateTime.UtcNow.AddHours(value: 48),
            LoginProvider = tokenId.ToString()
        };
    }

    /// <summary>
    ///     Sending user confirmation mail.
    /// </summary>
    /// <param name="command">
    ///     Request model.
    /// </param>
    /// <param name="emailConfirmedTokenValue">
    ///     email confirm token value.
    /// </param>
    /// <param name="ct">
    ///     The token to monitor cancellation requests.
    /// </param>
    /// <returns>
    ///     Nothing
    /// </returns>
    private async Task SendingUserConfirmationMailAsync(
        ResendUserConfirmationEmailRequest command,
        string emailConfirmedTokenValue,
        CancellationToken ct
    )
    {
        //Try to send mail.
        var sendingEmailCommand = new BackgroundJob.SendingUserConfirmationCommand
        {
            MainTokenValue = emailConfirmedTokenValue,
            Email = command.Email,
        };

        await _queueHandler.Value.QueueAsync(
            backgroundJobCommand: sendingEmailCommand,
            executeAfter: null,
            expireOn: DateTime.UtcNow.AddSeconds(value: 60),
            ct: ct
        );
    }
}
