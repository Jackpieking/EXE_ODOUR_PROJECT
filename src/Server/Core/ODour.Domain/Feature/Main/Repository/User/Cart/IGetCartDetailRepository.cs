using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ODour.Domain.Share.Cart.Entities;

namespace ODour.Domain.Feature.Main.Repository.User.Cart;

public interface IGetCartDetailRepository
{
    #region Query
    Task<bool> IsRefreshTokenFoundQueryAsync(string refreshTokenId, CancellationToken ct);

    Task<IEnumerable<CartItemEntity>> GetCartItemsOfUserQueryAsync(
        Guid userId,
        CancellationToken ct
    );
    #endregion
}
