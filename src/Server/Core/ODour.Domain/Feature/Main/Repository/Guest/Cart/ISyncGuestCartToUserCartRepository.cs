using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ODour.Domain.Share.Cart.Entities;
using ODour.Domain.Share.User.Entities;

namespace ODour.Domain.Feature.Main.Repository.Guest.Cart;

public interface ISyncGuestCartToUserCartRepository
{
    #region Query
    Task<IEnumerable<CartItemEntity>> FindCartByUserIdQueryAsync(Guid userId, CancellationToken ct);

    Task<UserTokenEntity> GetRefreshTokenQueryAsync(string refreshTokenId, CancellationToken ct);

    Task<bool> IsUserBannedQueryAsync(Guid userId, CancellationToken ct);
    #endregion

    #region Command
    Task<bool> UpdateUserCartCommandAsync(
        IEnumerable<CartItemEntity> newUserCartItems,
        IEnumerable<CartItemEntity> currentUserCartItems,
        Guid userId,
        CancellationToken ct
    );
    #endregion
}
