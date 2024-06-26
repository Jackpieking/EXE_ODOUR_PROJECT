using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ODour.Domain.Share.Product.Entities;

namespace ODour.Domain.Feature.Main.Repository.Guest.Cart;

public interface IGuestGetCartDetailRepository
{
    #region Query
    Task<IEnumerable<ProductEntity>> PopulateAllProductDetailOfCartQueryAsync(
        IEnumerable<string> productIds,
        CancellationToken ct
    );
    #endregion
}
