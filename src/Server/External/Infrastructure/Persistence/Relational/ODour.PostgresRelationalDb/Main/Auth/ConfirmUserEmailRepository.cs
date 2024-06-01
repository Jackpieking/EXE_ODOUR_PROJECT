using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ODour.Domain.Feature.Main.Repository.Auth;
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
        Guid userId,
        string tokenName,
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
                    // Get full user information.
                    var foundUser = await userManager.FindByIdAsync(userId: userId.ToString());

                    // Generate email confirm token.
                    var emailConfirmedToken = await userManager.GenerateEmailConfirmationTokenAsync(
                        user: foundUser
                    );

                    // Confirm user email.
                    var result = await userManager.ConfirmEmailAsync(
                        user: foundUser,
                        token: emailConfirmedToken
                    );

                    if (!result.Succeeded)
                    {
                        throw new DbUpdateConcurrencyException();
                    }

                    // Remove all email confirmed token of given user.
                    await _context
                        .Value.Set<UserTokenEntity>()
                        .Where(token => token.UserId == userId && token.Name.Equals(tokenName))
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

    public Task<UserTokenEntity> GetUserTokenByTokenIdQueryAsync(
        string tokenId,
        CancellationToken ct
    )
    {
        return _context
            .Value.Set<UserTokenEntity>()
            .AsNoTracking()
            .Where(predicate: token =>
                token.LoginProvider == tokenId && token.ExpiredAt > DateTime.UtcNow
            )
            .Select(selector: token => new UserTokenEntity
            {
                UserId = token.UserId,
                Name = token.Name
            })
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
            .Value.Set<UserEntity>()
            .AnyAsync(predicate: user => user.Id == userId, cancellationToken: ct);
    }

    public Task<bool> IsUserTemporarilyRemovedQueryAsync(Guid userId, CancellationToken ct)
    {
        return _context
            .Value.Set<UserEntity>()
            .AnyAsync(
                predicate: user => user.Id == userId && user.UserDetail.IsTemporarilyRemoved,
                cancellationToken: ct
            );
    }
}
