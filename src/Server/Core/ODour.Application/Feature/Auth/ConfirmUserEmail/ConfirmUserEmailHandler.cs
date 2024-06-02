using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        var (userId, tokenName) = await ValidateTokenAsync(token: command.Token, ct: ct);

        // Invalid token.
        if (userId == Guid.Empty)
        {
            return new() { StatusCode = ConfirmUserEmailResponseStatusCode.INVALID_TOKEN };
        }

        // Does user exist by user id.
        var isUserFound =
            await _unitOfWork.Value.ConfirmUserEmailRepository.IsUserFoundByUserIdQueryAsync(
                userId: userId,
                ct: ct
            );

        // User with email is not found.
        if (!isUserFound)
        {
            return new() { StatusCode = ConfirmUserEmailResponseStatusCode.USER_IS_NOT_FOUND };
        }

        // Has user confirmed email.
        var hasUserConfirmedEmail =
            await _unitOfWork.Value.ConfirmUserEmailRepository.HasUserConfirmedEmailQueryAsync(
                userId: userId,
                ct: ct
            );

        // User has confirmed email.
        if (hasUserConfirmedEmail)
        {
            return new()
            {
                StatusCode = ConfirmUserEmailResponseStatusCode.USER_HAS_CONFIRMED_EMAIL
            };
        }

        // Is user temporarily removed by email.
        var IsUserTemporarilyRemoved =
            await _unitOfWork.Value.ConfirmUserEmailRepository.IsUserTemporarilyRemovedQueryAsync(
                userId: userId,
                ct: ct
            );

        // User with email is temporarily removed.
        if (IsUserTemporarilyRemoved)
        {
            return new()
            {
                StatusCode = ConfirmUserEmailResponseStatusCode.USER_IS_TEMPORARILY_REMOVED
            };
        }

        // Getting successfully confirmed account status.
        var accountStatus =
            await _unitOfWork.Value.ConfirmUserEmailRepository.GetSuccesfullyConfirmedAccountStatusQueryAsync(
                ct: ct
            );

        // Confirm user email.
        var dbResult =
            await _unitOfWork.Value.ConfirmUserEmailRepository.ConfirmUserEmailCommandAsync(
                userId: userId,
                tokenName: tokenName,
                accountStatusId: accountStatus.Id,
                userManager: _userManager.Value,
                ct: ct
            );

        // Cannot confirm user email.
        if (!dbResult)
        {
            return new() { StatusCode = ConfirmUserEmailResponseStatusCode.OPERATION_FAIL };
        }

        return new() { StatusCode = ConfirmUserEmailResponseStatusCode.OPERATION_SUCCESS };
    }

    private async Task<(Guid, string)> ValidateTokenAsync(string token, CancellationToken ct)
    {
        var tokens = Encoding.UTF8.GetString(bytes: WebEncoders.Base64UrlDecode(input: token));

        if (string.IsNullOrWhiteSpace(value: tokens))
        {
            return (Guid.Empty, default);
        }

        var tokenId = tokens.Split(separator: CommonConstant.App.DefaultStringSeparator)[1];

        var foundUserToken =
            await _unitOfWork.Value.ConfirmUserEmailRepository.GetUserTokenByTokenIdQueryAsync(
                tokenId: tokenId,
                ct: ct
            );

        if (Equals(objA: foundUserToken, objB: default))
        {
            return (Guid.Empty, default);
        }

        return (foundUserToken.UserId, foundUserToken.Name);
    }
}
