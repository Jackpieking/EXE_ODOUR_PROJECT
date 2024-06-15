using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ODour.Domain.Share.Order.Entities;
using ODour.Domain.Share.User.Entities;

namespace ODour.Domain.Feature.Main.Repository.User.Order;

public interface IGetUserOrdersRepository
{
    #region Query
    Task<bool> IsOrderStatusFoundQueryAsync(Guid id, CancellationToken ct);

    Task<IEnumerable<OrderEntity>> GetAllOrderQueryAsync(
        Guid orderStatusId,
        Guid userId,
        CancellationToken ct
    );
    Task<UserTokenEntity> GetRefreshTokenQueryAsync(string refreshTokenId, CancellationToken ct);

    Task<bool> IsUserBannedQueryAsync(Guid userId, CancellationToken ct);
    #endregion
}
