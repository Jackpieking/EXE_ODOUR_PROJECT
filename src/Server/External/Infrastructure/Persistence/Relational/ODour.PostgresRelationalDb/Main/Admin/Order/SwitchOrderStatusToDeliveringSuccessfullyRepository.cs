using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ODour.Domain.Feature.Main.Repository.Admin.Order;
using ODour.Domain.Share.Order.Entities;
using ODour.Domain.Share.User.Entities;

namespace ODour.PostgresRelationalDb.Main.Admin.Order;

internal sealed class SwitchOrderStatusToDeliveringSuccessfullyRepository
    : ISwitchOrderStatusToDeliveringSuccessfullyRepository
{
    private readonly Lazy<DbContext> _context;

    public SwitchOrderStatusToDeliveringSuccessfullyRepository(Lazy<DbContext> context)
    {
        _context = context;
    }

    public Task<bool> IsRefreshTokenFoundQueryAsync(string refreshTokenId, CancellationToken ct)
    {
        return _context
            .Value.Set<UserTokenEntity>()
            .AnyAsync(
                predicate: token =>
                    token.LoginProvider.Equals(refreshTokenId) && token.ExpiredAt > DateTime.UtcNow,
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
                        input: "37eb8293-c17a-45a6-89ab-ba640d4001ff"
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
