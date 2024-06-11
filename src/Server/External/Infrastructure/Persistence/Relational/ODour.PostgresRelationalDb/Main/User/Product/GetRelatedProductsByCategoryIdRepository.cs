using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ODour.Domain.Feature.Main.Repository.User.Product;
using ODour.Domain.Share.Category.Entities;
using ODour.Domain.Share.Product.Entities;

namespace ODour.PostgresRelationalDb.Main.User.Product;

internal sealed class GetRelatedProductsByCategoryIdRepository
    : IGetRelatedProductsByCategoryIdRepository
{
    private readonly Lazy<DbContext> _context;

    public GetRelatedProductsByCategoryIdRepository(Lazy<DbContext> context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProductEntity>> GetRelatedProductsByCategoryIdQueryAsync(
        Guid categoryId,
        int numberOfProducts,
        CancellationToken ct
    )
    {
        // Get all product count.
        var allProductsCount = await _context
            .Value.Set<ProductEntity>()
            .CountAsync(
                predicate: product => product.CategoryId == categoryId,
                cancellationToken: ct
            );

        // No product found in specified category.
        if (allProductsCount == default)
        {
            return Enumerable.Empty<ProductEntity>();
        }

        // random a position of products [1 <= position <= allProductsCount]
        var productPosition =
            RandomNumberGenerator.GetInt32(fromInclusive: 1, toExclusive: allProductsCount + 1) - 1;

        return await _context
            .Value.Set<ProductEntity>()
            .AsNoTracking()
            .Where(predicate: product => product.CategoryId == categoryId)
            .Select(selector: product => new ProductEntity
            {
                Id = product.Id,
                Name = product.Name,
                UnitPrice = product.UnitPrice,
                ProductStatus = new() { Name = product.ProductStatus.Name },
                ProductMedias = product.ProductMedias.Select(image => new ProductMediaEntity
                {
                    StorageUrl = image.StorageUrl
                }),
                Category = new() { Id = product.Category.Id, Name = product.Category.Name }
            })
            .Skip(count: productPosition)
            .Take(count: numberOfProducts)
            .OrderBy(keySelector: _ => EF.Functions.Random())
            .ToListAsync(cancellationToken: ct);
    }

    public Task<bool> IsCategoryFoundByCategoryIdQueryAsync(Guid categoryId, CancellationToken ct)
    {
        return _context
            .Value.Set<CategoryEntity>()
            .AnyAsync(predicate: category => category.Id == categoryId, cancellationToken: ct);
    }
}
