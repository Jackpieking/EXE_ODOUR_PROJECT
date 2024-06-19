using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ODour.Domain.Feature.Main.Repository.User.Order;
using ODour.Domain.Share.Order.Entities;
using ODour.Domain.Share.Product.Entities;
using ODour.Domain.Share.User.Entities;

namespace ODour.PostgresRelationalDb.Main.User.Order;

internal sealed class GetUserOrdersRepository : IGetUserOrdersRepository
{
    private readonly Lazy<DbContext> _context;

    public GetUserOrdersRepository(Lazy<DbContext> context)
    {
        _context = context;
    }

    public Task<bool> IsRefreshTokenFoundQueryAsync(string refreshTokenId, CancellationToken ct)
    {
        return _context
            .Value.Set<UserTokenEntity>()
            .AnyAsync(
                predicate: token =>
                    token.LoginProvider.Equals(refreshTokenId) && token.ExpiredAt > DateTime.UtcNow,
                cancellationToken: ct
            );
    }

    public async Task<IEnumerable<OrderEntity>> GetAllOrderQueryAsync(
        Guid orderStatusId,
        Guid userId,
        CancellationToken ct
    )
    {
        return await _context
            .Value.Set<OrderEntity>()
            .AsNoTracking()
            .Where(predicate: entity =>
                entity.OrderStatusId == orderStatusId && entity.UserId == userId
            )
            .OrderByDescending(keySelector: entity => entity.OrderCode)
            .Select(selector: entity => new OrderEntity
            {
                Id = entity.Id,
                OrderStatus = new() { Name = entity.OrderStatus.Name },
                TotalPrice = entity.TotalPrice,
                OrderItems = entity.OrderItems.Select(orderItem => new OrderItemEntity
                {
                    ProductId = orderItem.ProductId,
                    SellingPrice = orderItem.SellingPrice,
                    SellingQuantity = orderItem.SellingQuantity,
                    Product = new()
                    {
                        Name = orderItem.Product.Name,
                        ProductMedias = orderItem
                            .Product.ProductMedias.OrderBy(productMedia => productMedia.UploadOrder)
                            .Skip(default)
                            .Take(1)
                            .Select(productMedia => new ProductMediaEntity
                            {
                                StorageUrl = productMedia.StorageUrl
                            })
                    }
                })
            })
            .ToListAsync(cancellationToken: ct);
    }

    public Task<bool> IsOrderStatusFoundQueryAsync(Guid id, CancellationToken ct)
    {
        return _context
            .Value.Set<OrderStatusEntity>()
            .AnyAsync(predicate: entity => entity.Id == id, cancellationToken: ct);
    }
}
