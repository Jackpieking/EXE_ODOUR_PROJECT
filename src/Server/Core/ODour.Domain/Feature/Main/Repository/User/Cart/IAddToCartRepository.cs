using System;
using System.Threading;
using System.Threading.Tasks;
using ODour.Domain.Share.Cart.Entities;
using ODour.Domain.Share.Product.Entities;

namespace ODour.Domain.Feature.Main.Repository.User.Cart;

public interface IAddToCartRepository
{
    #region Query
    Task<bool> IsRefreshTokenFoundQueryAsync(string refreshTokenId, CancellationToken ct);

    Task<ProductEntity> FindProductQueryAsync(string productId, CancellationToken ct);

    Task<bool> AddItemToCartQueryAsync(CartItemEntity cartItem, CancellationToken ct);

    Task<CartItemEntity> FindCartItemQueryAsync(
        string productId,
        Guid userId,
        CancellationToken ct
    );

    Task<int> GetTotalNumberOfCartItemQueryAsync(Guid userId, CancellationToken ct);

    Task<bool> UpdateQuantityQueryAsync(
        string productId,
        int newQuantity,
        Guid userId,
        CancellationToken ct
    );
    #endregion
}
