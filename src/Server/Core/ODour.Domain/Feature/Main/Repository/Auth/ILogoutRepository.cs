using System;
using System.Threading;
using System.Threading.Tasks;

namespace ODour.Domain.Feature.Main.Repository.Auth;

public interface ILogoutRepository
{
    #region Quries
    Task<bool> IsRefreshTokenFoundQueryAsync(
        string refreshToken,
        string refreshTokenId,
        CancellationToken ct
    );

    Task<bool> IsUserTemporarilyRemovedQueryAsync(Guid userId, CancellationToken ct);
    #endregion

    #region Commands
    Task<bool> RemoveRefreshTokenByItsValueCommandAsync(string refreshToken, CancellationToken ct);
    #endregion
}
