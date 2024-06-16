using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ODour.Domain.Feature.Main.Repository.User.Order;
using ODour.Domain.Share.Order.Entities;
using ODour.Domain.Share.User.Entities;

namespace ODour.PostgresRelationalDb.Main.User.Order;

internal sealed class GetUserOrdersRepository : IGetUserOrdersRepository
{
    private readonly Lazy<DbContext> _context;

    public GetUserOrdersRepository(Lazy<DbContext> context)
    {
        _context = context;
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

    public async Task<IEnumerable<OrderEntity>> GetAllOrderQueryAsync(
        Guid orderStatusId,
        Guid userId,
        CancellationToken ct
    )
    {
        return await _context
            .Value.Set<OrderEntity>()
            .AsNoTracking()
            .Where(predicate: entity =>
                entity.OrderStatusId == orderStatusId && entity.UserId == userId
            )
            .Select(selector: entity => new OrderEntity
            {
                OrderCode = entity.OrderCode,
                OrderStatus = new() { Name = entity.OrderStatus.Name },
                TotalPrice = entity.TotalPrice,
            })
            .OrderByDescending(keySelector: entity => entity.OrderCode)
            .ToListAsync(cancellationToken: ct);
    }

    public Task<bool> IsOrderStatusFoundQueryAsync(Guid id, CancellationToken ct)
    {
        return _context
            .Value.Set<OrderStatusEntity>()
            .AnyAsync(predicate: entity => entity.Id == id, cancellationToken: ct);
    }
}
