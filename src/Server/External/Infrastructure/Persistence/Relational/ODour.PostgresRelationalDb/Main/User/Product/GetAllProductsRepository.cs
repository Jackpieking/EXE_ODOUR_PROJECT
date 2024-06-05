using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ODour.Domain.Feature.Main.Repository.User.Product;
using ODour.Domain.Share.Product.Entities;

namespace ODour.PostgresRelationalDb.Main.User.Product;

internal sealed class GetAllProductsRepository : IGetAllProductsRepository
{
    private readonly Lazy<DbContext> _context;

    public GetAllProductsRepository(Lazy<DbContext> context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProductEntity>> GetAllProductsQueryAsync(
        int currentPage,
        int pageSize,
        CancellationToken ct
    )
    {
        int offset = (currentPage - 1) * pageSize;

        return await _context
            .Value.Set<ProductEntity>()
            .AsNoTracking()
            .Select(selector: product => new ProductEntity
            {
                Id = product.Id,
                Name = product.Name,
                UnitPrice = product.UnitPrice,
                Description = product.Description,
                QuantityInStock = product.QuantityInStock,
                ProductStatus = new() { Name = product.ProductStatus.Name }
            })
            .Skip(count: offset)
            .Take(count: pageSize)
            .ToListAsync(cancellationToken: ct);
    }

    public Task<int> GetProductsCountQueryAsync(CancellationToken ct)
    {
        return _context.Value.Set<ProductEntity>().CountAsync(cancellationToken: ct);
    }
}
