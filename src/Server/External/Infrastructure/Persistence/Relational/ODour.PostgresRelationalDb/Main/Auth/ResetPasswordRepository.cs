using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ODour.Domain.Feature.Main.Repository.Auth;
using ODour.Domain.Share.User.Entities;

namespace ODour.PostgresRelationalDb.Main.Auth;

internal sealed class ResetPasswordRepository : IResetPasswordRepository
{
    private readonly Lazy<DbContext> _context;

    internal ResetPasswordRepository(Lazy<DbContext> context)
    {
        _context = context;
    }

    public Task<UserTokenEntity> GetResetPasswordTokenByTokenIdQueryAsync(
        string tokenValue,
        CancellationToken ct
    )
    {
        return _context
            .Value.Set<UserTokenEntity>()
            .AsNoTracking()
            .Where(predicate: token =>
                token.Value.Equals(tokenValue) && token.ExpiredAt > DateTime.UtcNow
            )
            .Select(selector: token => new UserTokenEntity { UserId = token.UserId })
            .FirstOrDefaultAsync(cancellationToken: ct);
    }

    public Task<bool> IsUserFoundByUserIdQueryAsync(Guid userId, CancellationToken ct)
    {
        return _context
            .Value.Set<UserDetailEntity>()
            .AnyAsync(predicate: user => user.UserId == userId, cancellationToken: ct);
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

    public async Task<bool> ResetPasswordCommandAsync(
        UserEntity user,
        string tokenValue,
        string newPassword,
        UserManager<UserEntity> userManager,
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
                    // Reset user password.
                    var result = await userManager.ResetPasswordAsync(
                        user: user,
                        token: tokenValue,
                        newPassword: newPassword
                    );

                    if (!result.Succeeded)
                    {
                        throw new DbUpdateConcurrencyException();
                    }

                    // Remove all token of given user.
                    await _context
                        .Value.Set<UserTokenEntity>()
                        .Where(predicate: token =>
                            token.UserId == user.Id && token.Name.Equals("PasswordChanghingToken")
                        )
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
