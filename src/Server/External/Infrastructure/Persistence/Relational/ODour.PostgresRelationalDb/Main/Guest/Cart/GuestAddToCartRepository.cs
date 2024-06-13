using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ODour.Domain.Feature.Main.Repository.Guest.Cart;
using ODour.Domain.Share.Product.Entities;

namespace ODour.PostgresRelationalDb.Main.Guest.Cart;

internal sealed class GuestAddToCartRepository : IGuestAddToCartRepository
{
    private readonly Lazy<DbContext> _context;

    public GuestAddToCartRepository(Lazy<DbContext> context)
    {
        _context = context;
    }

    public Task<ProductEntity> GetProductQuantityInStockQueryAsync(
        string productId,
        CancellationToken ct
    )
    {
        return _context
            .Value.Set<ProductEntity>()
            .Where(predicate: product => product.Id.Equals(productId))
            .Select(selector: product => new ProductEntity
            {
                QuantityInStock = product.QuantityInStock
            })
            .FirstOrDefaultAsync(cancellationToken: ct);
    }
}
