using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ODour.Domain.Share.Order.Entities;
using ODour.Domain.Share.User.Entities;

namespace ODour.Domain.Feature.Main.Repository.User.Order;

public interface ICreateNewOrderRepository
{
    #region Query
    Task<bool> AreOrderItemsValidQueryAsync(
        IEnumerable<OrderItemEntity> orderItemEntities,
        Guid userId,
        CancellationToken ct
    );

    Task<OrderStatusEntity> GetOrderStatusBasedOnPaymentMethodQueryAsync(
        Guid paymentMethodId,
        CancellationToken ct
    );

    Task<bool> IsPaymentMethodValidQueryAsync(Guid paymentMethodId, CancellationToken ct);

    Task PopulateOrderItemQueryAsync(
        IEnumerable<OrderItemEntity> orderItems,
        Guid orderId,
        CancellationToken ct
    );

    Task<bool> IsUserBannedQueryAsync(Guid userId, CancellationToken ct);

    Task<UserTokenEntity> GetRefreshTokenQueryAsync(string refreshTokenId, CancellationToken ct);
    #endregion

    #region Command
    Task<bool> AddOrderCommandAsync(OrderEntity newOrder, CancellationToken ct);
    #endregion
}
