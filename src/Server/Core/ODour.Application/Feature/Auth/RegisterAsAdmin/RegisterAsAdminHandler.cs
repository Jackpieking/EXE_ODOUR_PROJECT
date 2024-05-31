using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.WebUtilities;
using ODour.Application.Share.Common;
using ODour.Application.Share.Features;
using ODour.Application.Share.Mail;
using ODour.Domain.Feature.Main;
using ODour.Domain.Share.System.Entities;

namespace ODour.Application.Feature.Auth.RegisterAsAdmin;

internal sealed class RegisterAsAdminHandler
    : IFeatureHandler<RegisterAsAdminRequest, RegisterAsAdminResponse>
{
    private readonly Lazy<IMainUnitOfWork> _unitOfWork;
    private readonly Lazy<ISendingMailHandler> _sendingMailHandler;
    private readonly Lazy<IDataProtectionProvider> dataProtectionProvider;

    public RegisterAsAdminHandler(
        Lazy<IMainUnitOfWork> unitOfWork,
        Lazy<ISendingMailHandler> sendingMailHandler
    )
    {
        _unitOfWork = unitOfWork;
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
    public async Task<RegisterAsAdminResponse> ExecuteAsync(
        RegisterAsAdminRequest command,
        CancellationToken ct
    )
    {
        // Does user exist by email.
        var isUserFound =
            await _unitOfWork.Value.RegisterAsAdminRepository.IsUserFoundByNormalizedEmailQueryAsync(
                email: command.Email,
                ct: ct
            );

        // User with email already exists.
        if (isUserFound)
        {
            return new() { StatusCode = RegisterAsAdminResponseStatusCode.USER_IS_EXISTED };
        }

        // Get account status for new user.
        var newAccountStatus =
            await _unitOfWork.Value.RegisterAsUserRepository.GetPendingConfirmedStatusQueryAsync(
                ct: ct
            );

        // Completing new user.
        InitAdmin(command: command, newAccountStatus: newAccountStatus.Id);

        // // Create and add user to role.
        // var dbResult =
        //     await _unitOfWork.Value.RegisterAsUserRepository.CreateAndAddUserToRoleCommandAsync(
        //         newUser: newUser,
        //         password: command.Password,
        //         userManager: _userManager.Value,
        //         ct: ct
        //     );

        // // Cannot create or add user to role.
        // if (!dbResult)
        // {
        //     return new() { StatusCode = RegisterAsAdminResponseStatusCode.OPERATION_FAIL };
        // }

        // // Getting mail content and sending.
        // var mainAccountConfirmedCode = WebEncoders.Base64UrlEncode(
        //     input: Encoding.UTF8.GetBytes(
        //         s: await _userManager.Value.GenerateEmailConfirmationTokenAsync(user: newUser)
        //     )
        // );

        // var alternateAccountConfirmedCode = WebEncoders.Base64UrlEncode(
        //     input: Encoding.UTF8.GetBytes(
        //         s: await _userManager.Value.GenerateEmailConfirmationTokenAsync(user: newUser)
        //     )
        // );

        // var mainContent =
        //     await _sendingMailHandler.Value.GetUserAccountConfirmationMailContentAsync(
        //         to: command.Email,
        //         subject: "Xác nhận tài khoản",
        //         mainLink: mainAccountConfirmedCode,
        //         alternateLink: alternateAccountConfirmedCode,
        //         cancellationToken: ct
        //     );

        // // Try to send mail.
        // var sendingUserConfirmationCommand = new BackgroundJob.SendingUserConfirmationCommand
        // {
        //     MailContent = mainContent
        // };

        // await sendingUserConfirmationCommand.QueueJobAsync(
        //     executeAfter: DateTime.UtcNow.AddSeconds(value: 5),
        //     expireOn: DateTime.UtcNow.AddMinutes(value: 5),
        //     ct: ct
        // );

        return new() { StatusCode = RegisterAsAdminResponseStatusCode.OPERATION_SUCCESS };
    }

    /// <summary>
    ///     Finishes filling the user with default
    ///     values for the newly created user.
    /// </summary>
    private static SystemAccountEntity InitAdmin(
        RegisterAsAdminRequest command,
        Guid newAccountStatus
    )
    {
        return new()
        {
            Id = Guid.NewGuid(),
            UserName = command.Email,
            NormalizedUserName = command.Email.ToUpper(),
            Email = command.Email,
            NormalizedEmail = command.Email.ToUpper(),
            //PasswordHash = protector.Protect(plaintext: "Admin123@"),
            AccessFailedCount = default,
            LockoutEnd = CommonConstant.App.MinTimeInUTC,
            AccountStatusId = newAccountStatus,
            SystemAccountRoles = new List<SystemAccountRoleEntity>
            {
                new() { RoleId = Guid.Parse(input: "c95f4aae-2a41-4c76-9cc4-f1d632409525"), }
            }
        };
    }
}
