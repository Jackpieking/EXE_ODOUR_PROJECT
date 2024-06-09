using System;
using System.Threading;
using System.Threading.Tasks;
using ODour.Domain.Share.User.Entities;

namespace ODour.Domain.Feature.Main.Repository.User.Cart;

public interface IGetCartDetailRepository
{
    #region Query
    Task<UserTokenEntity> GetRefreshTokenQueryAsync(string refreshTokenId, CancellationToken ct);

    Task<bool> IsUserBannedQueryAsync(Guid userId, CancellationToken ct);
    #endregion
}
