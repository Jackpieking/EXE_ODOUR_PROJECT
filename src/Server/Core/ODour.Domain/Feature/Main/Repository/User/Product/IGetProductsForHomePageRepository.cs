using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ODour.Domain.Share.Product.Entities;

namespace ODour.Domain.Feature.Main.Repository.User.Product;

public interface IGetProductsForHomePageRepository
{
    #region Query
    Task<IEnumerable<ProductEntity>> GetBestSellingProductsQueryAsync(
        int numberOfProducts,
        CancellationToken ct
    );

    Task<IEnumerable<ProductEntity>> GetNewProductsQueryAsync(
        int numberOfProducts,
        CancellationToken ct
    );
    #endregion
}
