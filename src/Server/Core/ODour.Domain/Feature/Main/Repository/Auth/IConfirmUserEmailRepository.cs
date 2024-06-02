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

    Task<bool> IsUserTemporarilyRemovedQueryAsync(Guid userId, CancellationToken ct);

    Task<bool> HasUserConfirmedEmailQueryAsync(Guid userId, CancellationToken ct);

    Task<AccountStatusEntity> GetSuccesfullyConfirmedAccountStatusQueryAsync(CancellationToken ct);

    Task<UserTokenEntity> GetUserTokenByTokenIdQueryAsync(string tokenId, CancellationToken ct);
    #endregion

    #region Command
    Task<bool> ConfirmUserEmailCommandAsync(
        Guid userId,
        string tokenName,
        Guid accountStatusId,
        UserManager<UserEntity> userManager,
        CancellationToken ct
    );
    #endregion
}
