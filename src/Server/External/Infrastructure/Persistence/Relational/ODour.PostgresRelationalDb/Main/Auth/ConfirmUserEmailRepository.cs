using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ODour.Domain.Feature.Main.Repository.Auth;
using ODour.Domain.Share.AccountStatus.Entities;
using ODour.Domain.Share.User.Entities;

namespace ODour.PostgresRelationalDb.Main.Auth;

internal sealed class ConfirmUserEmailRepository : IConfirmUserEmailRepository
{
    private readonly Lazy<DbContext> _context;

    internal ConfirmUserEmailRepository(Lazy<DbContext> context)
    {
        _context = context;
    }

    public async Task<bool> ConfirmUserEmailCommandAsync(
        UserEntity user,
        string tokenValue,
        Guid accountStatusId,
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
                    // Confirm user email.
                    var result = await userManager.ConfirmEmailAsync(user: user, token: tokenValue);

                    if (!result.Succeeded)
                    {
                        throw new DbUpdateConcurrencyException();
                    }

                    // Remove all email confirmed token of given user.
                    await _context
                        .Value.Set<UserTokenEntity>()
                        .Where(predicate: token =>
                            token.UserId.Equals(user.Id) && token.Name.Equals("EmailConfirmedToken")
                        )
                        .ExecuteDeleteAsync(cancellationToken: ct);

                    await _context
                        .Value.Set<UserDetailEntity>()
                        .Where(predicate: user => user.UserId == user.UserId)
                        .ExecuteUpdateAsync(
                            setPropertyCalls: builder =>
                                builder.SetProperty(user => user.AccountStatusId, accountStatusId),
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

    public Task<AccountStatusEntity> GetSuccesfullyConfirmedAccountStatusQueryAsync(
        CancellationToken ct
    )
    {
        return _context
            .Value.Set<AccountStatusEntity>()
            .AsNoTracking()
            .Where(predicate: entity => entity.Name.Equals("Đã xác nhận đăng ký"))
            .Select(selector: entity => new AccountStatusEntity { Id = entity.Id, })
            .FirstOrDefaultAsync(cancellationToken: ct);
    }

    public Task<UserTokenEntity> GetUserConfirmedEmailTokenByTokenIdQueryAsync(
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

    public Task<bool> HasUserConfirmedEmailQueryAsync(Guid userId, CancellationToken ct)
    {
        return _context
            .Value.Set<UserEntity>()
            .AnyAsync(
                predicate: user => user.Id == userId && user.EmailConfirmed,
                cancellationToken: ct
            );
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
}
