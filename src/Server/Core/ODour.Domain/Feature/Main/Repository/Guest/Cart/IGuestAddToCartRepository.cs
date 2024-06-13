using System.Threading;
using System.Threading.Tasks;
using ODour.Domain.Share.Product.Entities;

namespace ODour.Domain.Feature.Main.Repository.Guest.Cart;

public interface IGuestAddToCartRepository
{
    #region Query
    Task<ProductEntity> GetProductQuantityInStockQueryAsync(string productId, CancellationToken ct);
    #endregion
}
