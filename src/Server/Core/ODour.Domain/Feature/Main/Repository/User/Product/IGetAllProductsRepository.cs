using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ODour.Domain.Share.Product.Entities;

namespace ODour.Domain.Feature.Main.Repository.User.Product;

public interface IGetAllProductsRepository
{
    #region Query
    Task<IEnumerable<ProductEntity>> GetAllProductsQueryAsync(
        int currentPage,
        int pageSize,
        CancellationToken ct
    );

    Task<int> GetProductsCountQueryAsync(CancellationToken ct);
    #endregion
}
