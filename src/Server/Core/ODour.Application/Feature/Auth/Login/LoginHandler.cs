using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using ODour.Application.Share.Features;
using ODour.Application.Share.Tokens;
using ODour.Domain.Feature.Main;
using ODour.Domain.Share.User.Entities;

namespace ODour.Application.Feature.Auth.Login;

internal sealed class LoginHandler : IFeatureHandler<LoginRequest, LoginResponse>
{
    private readonly Lazy<IMainUnitOfWork> _unitOfWork;
    private readonly Lazy<UserManager<UserEntity>> _userManager;
    private readonly Lazy<SignInManager<UserEntity>> _signInManager;
    private readonly Lazy<IRefreshTokenHandler> _refreshTokenHandler;
    private readonly Lazy<IAccessTokenHandler> _accessTokenHandler;

    public LoginHandler(
        Lazy<IMainUnitOfWork> unitOfWork,
        Lazy<UserManager<UserEntity>> userManager,
        Lazy<SignInManager<UserEntity>> signInManager,
        Lazy<IRefreshTokenHandler> refreshTokenHandler,
        Lazy<IAccessTokenHandler> accessTokenHandler
    )
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _signInManager = signInManager;
        _refreshTokenHandler = refreshTokenHandler;
        _accessTokenHandler = accessTokenHandler;
    }

    public async Task<LoginResponse> ExecuteAsync(LoginRequest command, CancellationToken ct)
    {
        // Find user by email.
        var foundUser = await _userManager.Value.FindByNameAsync(userName: command.Email);

        // User with email does not exist.
        if (Equals(objA: foundUser, objB: default))
        {
            return new() { StatusCode = LoginResponseStatusCode.USER_IS_NOT_FOUND };
        }

        // Does password belong to user.
        var signInResult = await _signInManager.Value.CheckPasswordSignInAsync(
            user: foundUser,
            password: command.Password,
            lockoutOnFailure: true
        );

        // User password is uncorrect and number of login attempts is exceeded.
        if (!signInResult.Succeeded && signInResult.IsLockedOut)
        {
            return new() { StatusCode = LoginResponseStatusCode.USER_IS_TEMPORARILY_LOCKED_OUT };
        }
        // User password is uncorrect still can try to login again.
        else if (!signInResult.Succeeded)
        {
            return new() { StatusCode = LoginResponseStatusCode.USER_IS_NOT_FOUND };
        }

        // Is user temporarily banned by id.
        var IsUserTemporarilyRemoved =
            await _unitOfWork.Value.ConfirmUserEmailRepository.IsUserTemporarilyRemovedQueryAsync(
                userId: foundUser.Id,
                ct: ct
            );

        // User with id is temporarily banned.
        if (IsUserTemporarilyRemoved)
        {
            return new() { StatusCode = LoginResponseStatusCode.USER_IS_TEMPORARILY_BANNED };
        }

        // Get found user roles.
        var foundUserRoles = await _userManager.Value.GetRolesAsync(user: foundUser);

        // Init list of user claims.
        var userClaims = new List<Claim>
        {
            new(type: "jti", value: Guid.NewGuid().ToString()),
            new(type: "sub", value: foundUser.Id.ToString()),
            new(type: "role", value: foundUserRoles[default])
        };

        // Create new refresh token.
        var newRefreshToken = InitNewRefreshToken(
            userClaims: userClaims,
            isRememberMe: command.IsRememberMe
        );

        // Add new refresh token to the database.
        var dbResult = await _unitOfWork.Value.LoginRepository.CreateRefreshTokenCommandAsync(
            refreshToken: newRefreshToken,
            ct: ct
        );

        // Cannot add new refresh token to the database.
        if (!dbResult)
        {
            return new() { StatusCode = LoginResponseStatusCode.OPERATION_FAIL };
        }

        // Generate access token.
        var newAccessToken = _accessTokenHandler.Value.GenerateSigningToken(
            claims: userClaims,
            additionalSecondsFromNow: 600
        );

        foundUser.UserDetail =
            await _unitOfWork.Value.LoginRepository.GetUserInfoWithAvatarOnlyQueryAsync(
                userId: foundUser.Id,
                ct: ct
            );

        await SendingNotifyUserAboutLoginActionMailAsync(email: foundUser.Email, ct: ct);

        return new()
        {
            StatusCode = LoginResponseStatusCode.OPERATION_SUCCESS,
            Body = new()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Value,
                User = new()
                {
                    Email = foundUser.Email,
                    AvatarUrl = foundUser.UserDetail.AvatarUrl,
                    EmailConfirmed = foundUser.EmailConfirmed
                }
            }
        };
    }

    #region Others
    private static async Task SendingNotifyUserAboutLoginActionMailAsync(
        string email,
        CancellationToken ct
    )
    {
        // Try to send mail.
        var sendingAnyEmailCommand = new BackgroundJob.NotifyUserAboutLoginActionByEmailCommand
        {
            Email = email,
            CurrentTimeInUtc = DateTime.UtcNow
        };

        await sendingAnyEmailCommand.QueueJobAsync(
            expireOn: DateTime.UtcNow.AddMinutes(value: 5),
            ct: ct
        );
    }

    private UserTokenEntity InitNewRefreshToken(List<Claim> userClaims, bool isRememberMe)
    {
        return new()
        {
            LoginProvider = Guid.Parse(
                    input: userClaims
                        .First(predicate: claim => claim.Type.Equals(value: "jti"))
                        .Value
                )
                .ToString(),
            ExpiredAt = isRememberMe
                ? DateTime.UtcNow.AddDays(value: 15)
                : DateTime.UtcNow.AddDays(value: 3),
            UserId = Guid.Parse(
                input: userClaims.First(predicate: claim => claim.Type.Equals(value: "sub")).Value
            ),
            Value = _refreshTokenHandler.Value.Generate(length: 15),
            Name = "RefreshToken"
        };
    }
    #endregion
}
