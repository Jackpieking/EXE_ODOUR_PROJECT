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
        string refreshToken,
        string refreshTokenId,
        CancellationToken ct
    )
    {
        return _context
            .Value.Set<UserTokenEntity>()
            .AsNoTracking()
            .Where(predicate: token =>
                token.LoginProvider.Equals(refreshTokenId) && token.Value.Equals(refreshToken)
            )
            .Select(token => new UserTokenEntity { ExpiredAt = token.ExpiredAt })
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
}
