using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ODour.Domain.Feature.Main.Repository.User.Product;
using ODour.Domain.Share.Product.Entities;
using ODour.PostgresRelationalDb.Common;

namespace ODour.PostgresRelationalDb.Main.User.Product;

internal sealed class GetProductDetailByProductIdRepository : IGetProductDetailByProductIdRepository
{
    private readonly Lazy<DbContext> _context;

    public GetProductDetailByProductIdRepository(Lazy<DbContext> context)
    {
        _context = context;
    }

    public async Task<ProductEntity> GetProductDetailByProductIdQueryAsync(
        string productId,
        CancellationToken ct
    )
    {
        return await _context
            .Value.Set<ProductEntity>()
            .AsNoTracking()
            .Where(predicate: product =>
                EF.Functions.Collate(product.Id, CommonConstant.DatabaseCollation.CASE_INSENSITIVE)
                    .Equals(productId)
            )
            .Select(selector: product => new ProductEntity
            {
                Id = product.Id,
                Name = product.Name,
                UnitPrice = product.UnitPrice,
                Description = product.Description,
                QuantityInStock = product.QuantityInStock,
                ProductStatus = new() { Name = product.ProductStatus.Name },
                Category = new() { Id = product.Category.Id, Name = product.Category.Name },
                ProductMedias = product.ProductMedias.Select(image => new ProductMediaEntity
                {
                    UploadOrder = image.UploadOrder,
                    StorageUrl = image.StorageUrl
                })
            })
            .FirstOrDefaultAsync(cancellationToken: ct);
    }

    public Task<bool> IsProductFoundByProductIdQueryAsync(string productId, CancellationToken ct)
    {
        return _context
            .Value.Set<ProductEntity>()
            .AnyAsync(
                predicate: product =>
                    EF.Functions.Collate(
                            product.Id,
                            CommonConstant.DatabaseCollation.CASE_INSENSITIVE
                        )
                        .Equals(productId),
                cancellationToken: ct
            );
    }
}
