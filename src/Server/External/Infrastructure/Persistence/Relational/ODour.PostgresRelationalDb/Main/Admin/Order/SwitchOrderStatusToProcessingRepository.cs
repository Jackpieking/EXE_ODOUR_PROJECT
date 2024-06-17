﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ODour.Domain.Feature.Main.Repository.Admin.Order;
using ODour.Domain.Share.Order.Entities;
using ODour.Domain.Share.User.Entities;

namespace ODour.PostgresRelationalDb.Main.Admin.Order;

internal sealed class SwitchOrderStatusToProcessingRepository
    : ISwitchOrderStatusToProcessingRepository
{
    private readonly Lazy<DbContext> _context;

    public SwitchOrderStatusToProcessingRepository(Lazy<DbContext> context)
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

    public Task<bool> IsOrderFoundQueryAsync(Guid orderId, CancellationToken ct)
    {
        return _context
            .Value.Set<OrderEntity>()
            .AnyAsync(predicate: order => order.Id == orderId, cancellationToken: ct);
    }

    public async Task<bool> SwitchOrderStatusCommandAsync(Guid orderId, CancellationToken ct)
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
                    var processingOrderStatusId = Guid.Parse(
                        input: "9b1f0fcc-bdd4-4e57-9444-b19c3b2f0547"
                    );

                    await _context
                        .Value.Set<OrderEntity>()
                        .Where(predicate: order => order.Id == orderId)
                        .ExecuteUpdateAsync(
                            setPropertyCalls: builder =>
                                builder.SetProperty(
                                    order => order.OrderStatusId,
                                    processingOrderStatusId
                                ),
                            cancellationToken: ct
                        );

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

    public Task<OrderEntity> GetOrderCurrentInfoByOrderIdQueryAsync(
        Guid orderId,
        CancellationToken ct
    )
    {
        return _context
            .Value.Set<OrderEntity>()
            .AsNoTracking()
            .Where(predicate: order => order.Id == orderId)
            .Select(selector: order => new OrderEntity
            {
                UserId = order.UserId,
                OrderStatusId = order.OrderStatusId
            })
            .FirstOrDefaultAsync(cancellationToken: ct);
    }
}
