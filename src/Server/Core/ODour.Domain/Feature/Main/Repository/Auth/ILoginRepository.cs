using System;
using System.Threading;
using System.Threading.Tasks;
using ODour.Domain.Share.User.Entities;

namespace ODour.Domain.Feature.Main.Repository.Auth;

public interface ILoginRepository
{
    #region Query
    Task<UserDetailEntity> GetUserInfoWithAvatarOnlyQueryAsync(Guid userId, CancellationToken ct);

    Task<bool> IsUserTemporarilyRemovedQueryAsync(Guid userId, CancellationToken ct);
    #endregion

    #region Command
    Task<bool> CreateRefreshTokenCommandAsync(UserTokenEntity refreshToken, CancellationToken ct);
    #endregion
}
