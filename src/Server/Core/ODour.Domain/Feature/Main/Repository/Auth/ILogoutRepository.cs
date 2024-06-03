using System;
using System.Threading;
using System.Threading.Tasks;
using ODour.Domain.Share.User.Entities;

namespace ODour.Domain.Feature.Main.Repository.Auth;

public interface ILogoutRepository
{
    #region Quries
    Task<UserTokenEntity> GetRefreshTokenQueryAsync(string refreshTokenId, CancellationToken ct);

    Task<bool> IsUserBannedQueryAsync(Guid userId, CancellationToken ct);
    #endregion

    #region Commands
    Task<bool> RemoveRefreshTokenByItsValueCommandAsync(string refreshToken, CancellationToken ct);
    #endregion
}
