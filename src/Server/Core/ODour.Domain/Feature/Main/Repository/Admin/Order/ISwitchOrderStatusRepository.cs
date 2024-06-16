using System;
using System.Threading;
using System.Threading.Tasks;
using ODour.Domain.Share.Order.Entities;
using ODour.Domain.Share.User.Entities;

namespace ODour.Domain.Feature.Main.Repository.Admin.Order;

public interface ISwitchOrderStatusRepository
{
    #region Query
    Task<bool> IsUserBannedQueryAsync(Guid userId, CancellationToken ct);

    Task<UserTokenEntity> GetRefreshTokenQueryAsync(string refreshTokenId, CancellationToken ct);

    Task<bool> IsOrderFoundQueryAsync(Guid orderId, CancellationToken ct);

    Task<bool> IsOrderStatusFoundQueryAsync(Guid orderStatusId, CancellationToken ct);

    Task<OrderEntity> GetOrderCurrentInfoByOrderIdQueryAsync(Guid orderId, CancellationToken ct);
    #endregion

    #region Command
    Task<bool> SwitchOrderStatusCommandAsync(
        Guid newOrderStatusId,
        Guid orderId,
        CancellationToken ct
    );
    #endregion
}
