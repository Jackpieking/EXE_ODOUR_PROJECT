using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ODour.Domain.Feature.Main.Repository.Auth;
using ODour.Domain.Share.User.Entities;

namespace ODour.PostgresRelationalDb.Main.Auth;

internal sealed class LogoutRepository : ILogoutRepository
{
    private readonly Lazy<DbContext> _context;

    internal LogoutRepository(Lazy<DbContext> context)
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

    public async Task<bool> RemoveRefreshTokenCommandAsync(
        string refreshTokenId,
        CancellationToken ct
    )
    {
        var executedTransactionResult = false;

        await _context
            .Value.Database.CreateExecutionStrategy()
            .ExecuteAsync(operation: async () =>
            {
                await using var dbTransaction = await _context.Value.Database.BeginTransactionAsync(
                    cancellationToken: ct
                );

                try
                {
                    await _context
                        .Value.Set<UserTokenEntity>()
                        .Where(predicate: token => token.LoginProvider.Equals(refreshTokenId))
                        .ExecuteDeleteAsync(cancellationToken: ct);

                    await dbTransaction.CommitAsync(cancellationToken: ct);

                    executedTransactionResult = true;
                }
                catch
                {
                    await dbTransaction.RollbackAsync(cancellationToken: ct);
                }
            });

        return executedTransactionResult;
    }
}
