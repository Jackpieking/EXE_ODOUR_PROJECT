using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ODour.Domain.Share.AccountStatus.Entities;
using ODour.Domain.Share.User.Entities;

namespace ODour.Domain.Feature.Main.Repository.Auth;

public interface IConfirmUserEmailRepository
{
    #region Query
    Task<bool> IsUserFoundByUserIdQueryAsync(Guid userId, CancellationToken ct);

    Task<bool> IsUserBannedQueryAsync(Guid userId, CancellationToken ct);

    Task<bool> HasUserConfirmedEmailQueryAsync(Guid userId, CancellationToken ct);

    Task<AccountStatusEntity> GetSuccessfullyConfirmedAccountStatusQueryAsync(CancellationToken ct);

    Task<UserTokenEntity> GetUserConfirmedEmailTokenByTokenIdQueryAsync(
        string tokenValue,
        CancellationToken ct
    );
    #endregion

    #region Command
    Task<bool> ConfirmUserEmailCommandAsync(
        UserEntity user,
        string tokenValue,
        Guid accountStatusId,
        UserManager<UserEntity> userManager,
        CancellationToken ct
    );
    #endregion
}
