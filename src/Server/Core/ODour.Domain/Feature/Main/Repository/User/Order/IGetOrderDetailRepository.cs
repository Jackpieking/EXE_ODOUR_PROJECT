using System;
using System.Threading;
using System.Threading.Tasks;
using ODour.Domain.Share.Order.Entities;

namespace ODour.Domain.Feature.Main.Repository.User.Order;

public interface IGetOrderDetailRepository
{
    #region QUery
    Task<bool> IsRefreshTokenFoundQueryAsync(string refreshTokenId, CancellationToken ct);

    Task<bool> IsOrderFoundQueryAsync(Guid orderId, CancellationToken ct);

    Task<OrderEntity> GetOrderDetailQueryAsync(Guid orderId, CancellationToken ct);
    #endregion
}
