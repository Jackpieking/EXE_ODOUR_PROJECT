using System;
using System.Threading;
using System.Threading.Tasks;
using ODour.Domain.Share.Cart.Entities;
using ODour.Domain.Share.Product.Entities;
using ODour.Domain.Share.User.Entities;

namespace ODour.Domain.Feature.Main.Repository.User.Cart;

public interface IAddToCartRepository
{
    #region Query
    Task<UserTokenEntity> GetRefreshTokenQueryAsync(string refreshTokenId, CancellationToken ct);

    Task<bool> IsUserBannedQueryAsync(Guid userId, CancellationToken ct);

    Task<ProductEntity> FindProductQueryAsync(string productId, CancellationToken ct);

    Task<bool> IsInputValidQueryAsync(string productId, int quantity, CancellationToken ct);

    Task<bool> AddItemToCartQueryAsync(CartItemEntity cartItem, CancellationToken ct);

    Task<bool> IsCartItemFoundQueryAsync(string productId, Guid userId, CancellationToken ct);

    Task<bool> UpdateQuantityQueryAsync(
        string productId,
        int newQuantity,
        Guid userId,
        CancellationToken ct
    );
    #endregion
}
