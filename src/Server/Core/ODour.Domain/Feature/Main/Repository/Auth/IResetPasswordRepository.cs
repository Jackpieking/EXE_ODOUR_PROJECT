using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ODour.Domain.Share.User.Entities;

namespace ODour.Domain.Feature.Main.Repository.Auth;

public interface IResetPasswordRepository
{
    #region Query
    Task<bool> IsUserFoundByUserIdQueryAsync(Guid userId, CancellationToken ct);

    Task<bool> IsUserBannedQueryAsync(Guid userId, CancellationToken ct);

    Task<UserTokenEntity> GetResetPasswordTokenByTokenIdQueryAsync(
        string tokenValue,
        CancellationToken ct
    );
    #endregion

    #region Command
    Task<bool> ResetPasswordCommandAsync(
        UserEntity user,
        string tokenValue,
        string newPassword,
        UserManager<UserEntity> userManager,
        CancellationToken ct
    );
    #endregion
}
