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

namespace ODour.Application.Feature.Auth.ConfirmUserEmail;

internal sealed class ConfirmUserEmailHandler
    : IFeatureHandler<ConfirmUserEmailRequest, ConfirmUserEmailResponse>
{
    private readonly Lazy<IMainUnitOfWork> _unitOfWork;
    private readonly Lazy<UserManager<UserEntity>> _userManager;

    public ConfirmUserEmailHandler(
        Lazy<IMainUnitOfWork> unitOfWork,
        Lazy<UserManager<UserEntity>> userManager
    )
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<ConfirmUserEmailResponse> ExecuteAsync(
        ConfirmUserEmailRequest command,
        CancellationToken ct
    )
    {
        // Validate input token.
        #region InputValidation
        var (userId, tokenId, tokenValue) = await ValidateTokenAsync(token: command.Token, ct: ct);

        // Invalid token.
        if (userId == Guid.Empty)
        {
            return new() { StatusCode = ConfirmUserEmailResponseStatusCode.INPUT_VALIDATION_FAIL };
        }
        #endregion

        // Does user exist by user id.
        var isUserFound =
            await _unitOfWork.Value.ConfirmUserEmailRepository.IsUserFoundByUserIdQueryAsync(
                userId: userId,
                ct: ct
            );

        // User with id is not found.
        if (!isUserFound)
        {
            return new() { StatusCode = ConfirmUserEmailResponseStatusCode.USER_IS_NOT_FOUND };
        }

        // Has user confirmed id.
        var hasUserConfirmedEmail =
            await _unitOfWork.Value.ConfirmUserEmailRepository.HasUserConfirmedEmailQueryAsync(
                userId: userId,
                ct: ct
            );

        // User has confirmed id.
        if (hasUserConfirmedEmail)
        {
            return new()
            {
                StatusCode = ConfirmUserEmailResponseStatusCode.USER_HAS_CONFIRMED_EMAIL
            };
        }

        // Is user temporarily removed by id.
        var IsUserTemporarilyRemoved =
            await _unitOfWork.Value.ConfirmUserEmailRepository.IsUserBannedQueryAsync(
                userId: userId,
                ct: ct
            );

        // User with id is temporarily removed.
        if (IsUserTemporarilyRemoved)
        {
            return new()
            {
                StatusCode = ConfirmUserEmailResponseStatusCode.USER_IS_TEMPORARILY_BANNED
            };
        }

        // Getting successfully confirmed account status.
        var accountStatus =
            await _unitOfWork.Value.ConfirmUserEmailRepository.GetSuccesfullyConfirmedAccountStatusQueryAsync(
                ct: ct
            );

        // Get full user info.
        var foundUser = await _userManager.Value.FindByIdAsync(userId: userId.ToString());

        // Confirm user email.
        var dbResult =
            await _unitOfWork.Value.ConfirmUserEmailRepository.ConfirmUserEmailCommandAsync(
                user: foundUser,
                tokenId: tokenId,
                tokenValue: tokenValue,
                accountStatusId: accountStatus.Id,
                userManager: _userManager.Value,
                ct: ct
            );

        // Cannot confirm user email.
        if (!dbResult)
        {
            return new() { StatusCode = ConfirmUserEmailResponseStatusCode.OPERATION_FAIL };
        }

        await SendingUserConfirmationSuccessfullyMailAsync(email: foundUser.Email, ct: ct);

        return new() { StatusCode = ConfirmUserEmailResponseStatusCode.OPERATION_SUCCESS };
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
            await _unitOfWork.Value.ConfirmUserEmailRepository.GetUserConfirmedEmailTokenByTokenIdQueryAsync(
                tokenId: tokens[1],
                ct: ct
            );

        if (Equals(objA: foundUserToken, objB: default))
        {
            return (Guid.Empty, Guid.Empty, string.Empty);
        }

        return (foundUserToken.UserId, Guid.Parse(input: tokens[1]), tokens[0]);
    }

    private static async Task SendingUserConfirmationSuccessfullyMailAsync(
        string email,
        CancellationToken ct
    )
    {
        // Try to send mail.
        var sendingAnyEmailCommand = new BackgroundJob.SendUserConfirmSuccessfullyEmailCommand
        {
            Email = email
        };

        await sendingAnyEmailCommand.QueueJobAsync(
            expireOn: DateTime.UtcNow.AddMinutes(value: 5),
            ct: ct
        );
    }
}
