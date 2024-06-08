using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ODour.Domain.Share.Product.Entities;

namespace ODour.Domain.Feature.Main.Repository.User.Product;

public interface IGetRelatedProductsByCategoryIdRepository
{
    #region Query
    Task<bool> IsCategoryFoundByCategoryIdQueryAsync(Guid categoryId, CancellationToken ct);

    Task<IEnumerable<ProductEntity>> GetRelatedProductsByCategoryIdQueryAsync(
        Guid categoryId,
        int numberOfProducts,
        CancellationToken ct
    );
    #endregion
}
