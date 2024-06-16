using System;
using System.Threading;
using System.Threading.Tasks;
using ODour.Domain.Share.Order.Entities;
using ODour.Domain.Share.User.Entities;

namespace ODour.Domain.Feature.Main.Repository.User.Order;

public interface IGetOrderDetailRepository
{
    #region QUery
    Task<bool> IsUserBannedQueryAsync(Guid userId, CancellationToken ct);

    Task<UserTokenEntity> GetRefreshTokenQueryAsync(string refreshTokenId, CancellationToken ct);

    Task<bool> IsOrderFoundQueryAsync(Guid orderId, CancellationToken ct);

    Task<OrderEntity> GetOrderDetailQueryAsync(Guid orderId, CancellationToken ct);
    #endregion
}
