using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ODour.Domain.Share.Order.Entities;

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
    Task<bool> IsRefreshTokenFoundQueryAsync(string refreshTokenId, CancellationToken ct);
    #endregion
}
