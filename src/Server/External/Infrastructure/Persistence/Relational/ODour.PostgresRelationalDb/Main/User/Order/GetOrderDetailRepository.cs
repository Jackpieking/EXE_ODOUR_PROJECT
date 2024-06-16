using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ODour.Domain.Feature.Main.Repository.User.Order;
using ODour.Domain.Share.Order.Entities;
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
                    Product = new() { Name = orderItem.Product.Name }
                })
            })
            .FirstOrDefaultAsync(cancellationToken: ct);
    }

    public Task<UserTokenEntity> GetRefreshTokenQueryAsync(
        string refreshTokenId,
        CancellationToken ct
    )
    {
        return _context
            .Value.Set<UserTokenEntity>()
            .AsNoTracking()
            .Where(predicate: token => token.LoginProvider.Equals(refreshTokenId))
            .Select(token => new UserTokenEntity
            {
                Value = token.Value,
                ExpiredAt = token.ExpiredAt
            })
            .FirstOrDefaultAsync(cancellationToken: ct);
    }

    public Task<bool> IsOrderFoundQueryAsync(Guid OrderId, CancellationToken ct)
    {
        return _context
            .Value.Set<OrderEntity>()
            .AnyAsync(predicate: order => order.Id == OrderId, cancellationToken: ct);
    }

    public Task<bool> IsUserBannedQueryAsync(Guid userId, CancellationToken ct)
    {
        return _context
            .Value.Set<UserDetailEntity>()
            .AnyAsync(
                predicate: user =>
                    user.UserId == userId
                    && user.AccountStatus.Name.Equals("Bị cấm trong hệ thống"),
                cancellationToken: ct
            );
    }
}
