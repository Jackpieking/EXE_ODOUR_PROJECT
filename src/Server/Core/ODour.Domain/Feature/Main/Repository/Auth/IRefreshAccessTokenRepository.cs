using System.Threading;
using System.Threading.Tasks;
using ODour.Domain.Share.User.Entities;

namespace ODour.Domain.Feature.Main.Repository.Auth;

public interface IRefreshAccessTokenRepository
{
    #region Quries
    Task<UserTokenEntity> GetRefreshTokenQueryAsync(
        string refreshTokenId,
        string refreshTokenValue,
        CancellationToken ct
    );
    #endregion

    #region Commands
    Task<bool> UpdateRefreshTokenQueryAsync(
        string oldRefreshTokenId,
        string newRefreshTokenId,
        CancellationToken ct
    );
    #endregion
}
