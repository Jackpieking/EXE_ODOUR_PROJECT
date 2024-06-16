using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ODour.Domain.Feature.Main.Repository.User.Order;
using ODour.Domain.Share.Cart.Entities;
using ODour.Domain.Share.Order.Entities;
using ODour.Domain.Share.Payment.Entities;
using ODour.Domain.Share.Product.Entities;
using ODour.Domain.Share.User.Entities;

namespace ODour.PostgresRelationalDb.Main.User.Order;

internal sealed class CreateNewOrderRepository : ICreateNewOrderRepository
{
    private readonly Lazy<DbContext> _context;

    public CreateNewOrderRepository(Lazy<DbContext> context)
    {
        _context = context;
    }

    public async Task<bool> AreOrderItemsValidQueryAsync(
        IEnumerable<OrderItemEntity> orderItemEntities,
        Guid userId,
        CancellationToken ct
    )
    {
        foreach (var orderItem in orderItemEntities)
        {
            var isValid = await _context
                .Value.Set<CartItemEntity>()
                .AsNoTracking()
                .AnyAsync(
                    predicate: cartItem =>
                        cartItem.UserId == userId
                        && cartItem.ProductId.Equals(orderItem.ProductId)
                        && cartItem.Quantity >= orderItem.SellingQuantity,
                    cancellationToken: ct
                );

            if (!isValid)
            {
                return false;
            }
        }

        return true;
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

    public async Task<OrderStatusEntity> GetOrderStatusBasedOnPaymentMethodQueryAsync(
        Guid paymentMethodId,
        CancellationToken ct
    )
    {
        var isPayWhenReceived = await _context
            .Value.Set<PaymentMethodEntity>()
            .AnyAsync(
                predicate: entity =>
                    entity.Id == paymentMethodId && entity.Name.Equals("Thanh toán khi nhận hàng"),
                cancellationToken: ct
            );

        if (isPayWhenReceived)
        {
            return await _context
                .Value.Set<OrderStatusEntity>()
                .AsNoTracking()
                .Where(predicate: entity => entity.Name.Equals("Chờ xử lý"))
                .Select(selector: entity => new OrderStatusEntity { Id = entity.Id })
                .FirstOrDefaultAsync(cancellationToken: ct);
        }

        return await _context
            .Value.Set<OrderStatusEntity>()
            .AsNoTracking()
            .Where(predicate: entity => entity.Name.Equals("Chờ thanh toán"))
            .Select(selector: entity => new OrderStatusEntity { Id = entity.Id })
            .FirstOrDefaultAsync(cancellationToken: ct);
    }

    public Task<bool> IsPaymentMethodValidQueryAsync(Guid paymentMethodId, CancellationToken ct)
    {
        return _context
            .Value.Set<PaymentMethodEntity>()
            .AnyAsync(predicate: entity => entity.Id == paymentMethodId, cancellationToken: ct);
    }

    public async Task PopulateOrderItemQueryAsync(
        IEnumerable<OrderItemEntity> orderItems,
        Guid orderId,
        CancellationToken ct
    )
    {
        foreach (var orderItem in orderItems)
        {
            orderItem.OrderId = orderId;

            var foundProduct = await _context
                .Value.Set<ProductEntity>()
                .AsNoTracking()
                .Where(predicate: entity => entity.Id.Equals(orderItem.ProductId))
                .Select(selector: entity => new ProductEntity { UnitPrice = entity.UnitPrice })
                .FirstOrDefaultAsync(cancellationToken: ct);

            orderItem.SellingPrice = foundProduct.UnitPrice;
        }
    }

    public async Task<bool> AddOrderCommandAsync(OrderEntity newOrder, CancellationToken ct)
    {
        var dbResult = false;

        await _context
            .Value.Database.CreateExecutionStrategy()
            .ExecuteAsync(operation: async () =>
            {
                await using var dbTransaction = await _context.Value.Database.BeginTransactionAsync(
                    cancellationToken: ct
                );

                try
                {
                    // Update cart items
                    foreach (var orderItem in newOrder.OrderItems)
                    {
                        await _context
                            .Value.Set<CartItemEntity>()
                            .Where(predicate: cartItem =>
                                cartItem.UserId == newOrder.UserId
                                && cartItem.ProductId == cartItem.ProductId
                            )
                            .ExecuteUpdateAsync(
                                setPropertyCalls: builder =>
                                    builder.SetProperty(
                                        cartItem => cartItem.Quantity,
                                        cartItem => cartItem.Quantity - orderItem.SellingQuantity
                                    ),
                                cancellationToken: ct
                            );
                    }

                    // Remove all cart items with quantity = 0
                    await _context
                        .Value.Set<CartItemEntity>()
                        .Where(predicate: cartItem =>
                            cartItem.UserId == newOrder.UserId && cartItem.Quantity == default
                        )
                        .ExecuteDeleteAsync(cancellationToken: ct);

                    // Add new order
                    await _context
                        .Value.Set<OrderEntity>()
                        .AddAsync(entity: newOrder, cancellationToken: ct);

                    await _context.Value.SaveChangesAsync(cancellationToken: ct);

                    await dbTransaction.CommitAsync(cancellationToken: ct);

                    dbResult = true;
                }
                catch
                {
                    await dbTransaction.RollbackAsync(cancellationToken: ct);
                }
            });

        return dbResult;
    }
}
