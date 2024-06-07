using System.Threading;
using System.Threading.Tasks;
using ODour.Domain.Share.Product.Entities;

namespace ODour.Domain.Feature.Main.Repository.User.Product;

public interface IGetProductDetailByProductIdRepository
{
    #region Query
    Task<ProductEntity> GetProductDetailByProductIdQueryAsync(
        string productId,
        CancellationToken ct
    );

    Task<bool> IsProductFoundByProductIdQueryAsync(string productId, CancellationToken ct);
    #endregion
}
