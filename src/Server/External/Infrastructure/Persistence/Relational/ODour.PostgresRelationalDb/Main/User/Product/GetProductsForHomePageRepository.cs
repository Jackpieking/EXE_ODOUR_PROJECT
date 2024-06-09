using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ODour.Domain.Feature.Main.Repository.User.Product;
using ODour.Domain.Share.Events;
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
        var foundOrders = await _context
            .Value.Set<OrderEntity>()
            .AsNoTracking()
            .Where(predicate: order => order.OrderStatus.Name.Equals("Giao hàng thành công"))
            .Select(selector: order => new OrderEntity
            {
                OrderItems = order.OrderItems.Select(orderItem => new OrderItemEntity
                {
                    Product = new()
                    {
                        Id = orderItem.Product.Id,
                        Name = orderItem.Product.Name,
                        UnitPrice = orderItem.Product.UnitPrice,
                        ProductStatus = new() { Name = orderItem.Product.ProductStatus.Name },
                        ProductMedias = orderItem.Product.ProductMedias.Select(
                            image => new ProductMediaEntity { StorageUrl = image.StorageUrl }
                        ),
                        Category = new()
                        {
                            Id = orderItem.Product.Category.Id,
                            Name = orderItem.Product.Category.Name
                        }
                    }
                })
            })
            .ToListAsync(cancellationToken: ct);

        var sellProducts = new Dictionary<string, (ProductEntity productDetail, int sellCount)>();

        // construct a mapper with key as product id and value as sell count
        foreach (var order in foundOrders)
        {
            foreach (var orderItem in order.OrderItems)
            {
                if (sellProducts.TryGetValue(orderItem.Product.Id, out var infoBag))
                {
                    infoBag.sellCount += 1;
                }
                else
                {
                    sellProducts.Add(key: orderItem.Product.Id, value: (infoBag.productDetail, 1));
                }
            }
        }

        sellProducts.TrimExcess();

        return sellProducts
            .OrderBy(keySelector: product => product.Value.sellCount)
            .Select(selector: product => product.Value.productDetail);
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
