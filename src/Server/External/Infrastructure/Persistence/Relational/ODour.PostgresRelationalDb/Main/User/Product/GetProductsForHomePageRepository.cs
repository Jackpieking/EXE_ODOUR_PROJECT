using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ODour.Domain.Feature.Main.Repository.User.Product;
using ODour.Domain.Share.Order.Entities;
using ODour.Domain.Share.Product.Entities;

namespace ODour.PostgresRelationalDb.Main.User.Product;

internal sealed class GetProductsForHomePageRepository : IGetProductsForHomePageRepository
{
    private readonly Lazy<DbContext> _context;

    public GetProductsForHomePageRepository(Lazy<DbContext> context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProductEntity>> GetBestSellingProductsQueryAsync(
        int numberOfProducts,
        CancellationToken ct
    )
    {
        var sellProducts = await _context
            .Value.Set<OrderItemEntity>()
            .AsNoTracking()
            .Include(navigationPropertyPath: orderItem => orderItem.Product)
            .ThenInclude(navigationPropertyPath: product => product.ProductStatus)
            .Include(navigationPropertyPath: orderItem => orderItem.Product)
            .ThenInclude(navigationPropertyPath: product => product.ProductMedias)
            .Include(navigationPropertyPath: orderItem => orderItem.Product)
            .ThenInclude(navigationPropertyPath: product => product.Category)
            .Where(predicate: orderItem =>
                orderItem.Order.OrderStatus.Name.Equals("Giao hàng thành công")
            )
            .GroupBy(keySelector: orderItem => new
            {
                orderItem.Product.Id,
                orderItem.Product.Name,
                orderItem.Product.UnitPrice,
                ProductStatusName = orderItem.Product.ProductStatus.Name,
                CategoryId = orderItem.Product.Category.Id,
                CategoryName = orderItem.Product.Category.Name
            })
            .Select(selector: group => new
            {
                ProductDetail = new ProductEntity
                {
                    Id = group.Key.Id,
                    Name = group.Key.Name,
                    UnitPrice = group.Key.UnitPrice,
                    ProductStatus = new() { Name = group.Key.ProductStatusName },
                    ProductMedias = group
                        .First()
                        .Product.ProductMedias.Select(image => new ProductMediaEntity
                        {
                            StorageUrl = image.StorageUrl
                        }),
                    Category = new() { Id = group.Key.CategoryId, Name = group.Key.CategoryName }
                },
                SellCount = group.Count()
            })
            .OrderByDescending(keySelector: product => product.SellCount)
            .ToDictionaryAsync(
                keySelector: product => product.ProductDetail.Id,
                elementSelector: product => (product.ProductDetail, product.SellCount),
                cancellationToken: ct
            );

        return sellProducts.Select(selector: product => product.Value.ProductDetail);
    }

    public async Task<IEnumerable<ProductEntity>> GetNewProductsQueryAsync(
        int numberOfProducts,
        CancellationToken ct
    )
    {
        var foundProductCount = await _context
            .Value.Set<ProductEntity>()
            .CountAsync(cancellationToken: ct);

        return await _context
            .Value.Set<ProductEntity>()
            .AsNoTracking()
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
            .Skip(count: foundProductCount - numberOfProducts)
            .Take(count: numberOfProducts)
            .ToListAsync(cancellationToken: ct);
    }
}
