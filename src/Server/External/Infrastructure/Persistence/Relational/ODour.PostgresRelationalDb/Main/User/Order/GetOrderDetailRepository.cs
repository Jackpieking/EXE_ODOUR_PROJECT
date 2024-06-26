﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ODour.Domain.Feature.Main.Repository.User.Order;
using ODour.Domain.Share.Order.Entities;
using ODour.Domain.Share.Product.Entities;
using ODour.Domain.Share.User.Entities;

namespace ODour.PostgresRelationalDb.Main.User.Order;

internal sealed class GetOrderDetailRepository : IGetOrderDetailRepository
{
    private readonly Lazy<DbContext> _context;

    public GetOrderDetailRepository(Lazy<DbContext> context)
    {
        _context = context;
    }

    public Task<OrderEntity> GetOrderDetailQueryAsync(Guid orderId, CancellationToken ct)
    {
        return _context
            .Value.Set<OrderEntity>()
            .AsNoTracking()
            .Where(predicate: order => order.Id == orderId)
            .Select(selector: order => new OrderEntity
            {
                OrderStatus = new() { Name = order.OrderStatus.Name },
                PaymentMethod = new() { Name = order.PaymentMethod.Name },
                FullName = order.FullName,
                DeliveredAddress = order.DeliveredAddress,
                PhoneNumber = order.PhoneNumber,
                OrderNote = order.OrderNote,
                DeliveredAt = order.DeliveredAt,
                TotalPrice = order.TotalPrice,
                OrderItems = order.OrderItems.Select(orderItem => new OrderItemEntity
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
            .FirstOrDefaultAsync(cancellationToken: ct);
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

    public Task<bool> IsOrderFoundQueryAsync(Guid orderId, CancellationToken ct)
    {
        return _context
            .Value.Set<OrderEntity>()
            .AnyAsync(predicate: order => order.Id == orderId, cancellationToken: ct);
    }
}
