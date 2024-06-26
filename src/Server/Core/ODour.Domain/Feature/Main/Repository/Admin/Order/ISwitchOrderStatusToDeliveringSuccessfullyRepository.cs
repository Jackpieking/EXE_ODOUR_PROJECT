using System;
using System.Threading;
using System.Threading.Tasks;
using ODour.Domain.Share.Order.Entities;

namespace ODour.Domain.Feature.Main.Repository.Admin.Order;

public interface ISwitchOrderStatusToDeliveringSuccessfullyRepository
{
    #region Query
    Task<bool> IsRefreshTokenFoundQueryAsync(string refreshTokenId, CancellationToken ct);

    Task<bool> IsOrderFoundQueryAsync(Guid orderId, CancellationToken ct);

    Task<OrderEntity> GetOrderCurrentInfoByOrderIdQueryAsync(Guid orderId, CancellationToken ct);
    #endregion

    #region Command
    Task<bool> SwitchOrderStatusCommandAsync(Guid orderId, CancellationToken ct);
    #endregion
}
