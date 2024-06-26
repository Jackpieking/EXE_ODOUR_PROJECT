using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ODour.Domain.Feature.Main.Repository.Guest.Cart;
using ODour.Domain.Share.Product.Entities;

namespace ODour.PostgresRelationalDb.Main.Guest.Cart;

internal sealed class GuestGetCartDetailRepository : IGuestGetCartDetailRepository
{
    private readonly Lazy<DbContext> _context;

    internal GuestGetCartDetailRepository(Lazy<DbContext> context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProductEntity>> PopulateAllProductDetailOfCartQueryAsync(
        IEnumerable<string> productIds,
        CancellationToken ct
    )
    {
        return await _context
            .Value.Set<ProductEntity>()
            .AsNoTracking()
            .Where(predicate: product => productIds.Contains(product.Id))
            .Select(selector: product => new ProductEntity
            {
                Id = product.Id,
                Name = product.Name,
                UnitPrice = product.UnitPrice
            })
            .ToListAsync(cancellationToken: ct);
    }
}
