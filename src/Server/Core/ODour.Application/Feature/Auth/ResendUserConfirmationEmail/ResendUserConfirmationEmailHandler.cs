using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using ODour.Application.Share.Features;
using ODour.Domain.Feature.Main;
using ODour.Domain.Share.User.Entities;

namespace ODour.Application.Feature.Auth.ResendUserConfirmationEmail;

internal sealed class ResendUserConfirmationEmailHandler
    : IFeatureHandler<ResendUserConfirmationEmailRequest, ResendUserConfirmationEmailResponse>
{
    private readonly Lazy<IMainUnitOfWork> _unitOfWork;
    private readonly Lazy<UserManager<UserEntity>> _userManager;

    public ResendUserConfirmationEmailHandler(
        Lazy<IMainUnitOfWork> unitOfWork,
        Lazy<UserManager<UserEntity>> userManager
    )
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
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
        var emailConfirmedTokens = await GenerateUserEmailConfirmedTokenAsync(user: user);

        // Add email confirm token to the database.
        var dbResult =
            await _unitOfWork.Value.ResendUserConfirmationEmailRepository.AddUserPasswordChangingTokenCommandAsync(
                userTokenEntities: new List<UserTokenEntity> { emailConfirmedTokens["MainToken"] },
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

    private async Task<Dictionary<string, UserTokenEntity>> GenerateUserEmailConfirmedTokenAsync(
        UserEntity user
    )
    {
        Dictionary<string, UserTokenEntity> emailConfirmedTokens = new(capacity: 2);

        var tokenId = Guid.NewGuid();

        // Add new token for email confirmed.
        emailConfirmedTokens.Add(
            key: "MainToken",
            value: new()
            {
                UserId = user.Id,
                Name = "EmailConfirmedToken",
                Value = await _userManager.Value.GenerateEmailConfirmationTokenAsync(user: user),
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
        ResendUserConfirmationEmailRequest command,
        Dictionary<string, UserTokenEntity> emailConfirmedTokens,
        CancellationToken ct
    )
    {
        //Try to send mail.
        var sendingEmailEvent = new BackgroundJob.SendingUserConfirmationEvent
        {
            MainTokenValue = emailConfirmedTokens["MainToken"].Value,
            Email = command.Email,
        };

        await sendingEmailEvent.PublishAsync(waitMode: Mode.WaitForNone, cancellation: ct);
    }
}
