using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ODour.Domain.Feature.Main.Repository.Auth;
using ODour.Domain.Share.User.Entities;

namespace ODour.PostgresRelationalDb.Main.Auth;

internal sealed class RefreshAccessTokenRepository : IRefreshAccessTokenRepository
{
    private readonly Lazy<DbContext> _context;

    internal RefreshAccessTokenRepository(Lazy<DbContext> context)
    {
        _context = context;
    }

    public async Task<bool> UpdateRefreshTokenQueryAsync(
        string oldRefreshTokenId,
        string newRefreshTokenId,
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
                        .Where(predicate: token => token.LoginProvider.Equals(oldRefreshTokenId))
                        .ExecuteUpdateAsync(
                            setPropertyCalls: builder =>
                                builder.SetProperty(
                                    token => token.LoginProvider,
                                    newRefreshTokenId
                                ),
                            cancellationToken: ct
                        );

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

    public Task<UserTokenEntity> GetRefreshTokenQueryAsync(
        string refreshTokenId,
        string refreshTokenValue,
        CancellationToken ct
    )
    {
        return _context
            .Value.Set<UserTokenEntity>()
            .AsNoTracking()
            .Where(predicate: token =>
                token.LoginProvider.Equals(refreshTokenId)
                && token.ExpiredAt > DateTime.UtcNow
                && token.Value.Equals(refreshTokenValue)
            )
            .Select(token => new UserTokenEntity { Value = token.Value })
            .FirstOrDefaultAsync(cancellationToken: ct);
    }
}
