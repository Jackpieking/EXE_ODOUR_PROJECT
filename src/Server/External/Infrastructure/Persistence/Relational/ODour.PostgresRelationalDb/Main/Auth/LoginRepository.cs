using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ODour.Domain.Feature.Main.Repository.Auth;
using ODour.Domain.Share.User.Entities;

namespace ODour.PostgresRelationalDb.Main.Auth;

internal sealed class LoginRepository : ILoginRepository
{
    private readonly Lazy<DbContext> _context;

    internal LoginRepository(Lazy<DbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateRefreshTokenCommandAsync(
        UserTokenEntity refreshToken,
        CancellationToken ct
    )
    {
        try
        {
            await _context
                .Value.Set<UserTokenEntity>()
                .AddAsync(entity: refreshToken, cancellationToken: ct);

            await _context.Value.SaveChangesAsync(cancellationToken: ct);
        }
        catch
        {
            return false;
        }

        return true;
    }

    public Task<UserDetailEntity> GetUserInfoWithAvatarOnlyQueryAsync(
        Guid userId,
        CancellationToken ct
    )
    {
        return _context
            .Value.Set<UserDetailEntity>()
            .AsNoTracking()
            .Where(user => user.UserId == userId)
            .Select(user => new UserDetailEntity { AvatarUrl = user.AvatarUrl })
            .FirstOrDefaultAsync(cancellationToken: ct);
    }

    public Task<bool> IsUserTemporarilyRemovedQueryAsync(Guid userId, CancellationToken ct)
    {
        return _context
            .Value.Set<UserDetailEntity>()
            .AnyAsync(
                predicate: user => user.UserId == userId && user.IsTemporarilyRemoved,
                cancellationToken: ct
            );
    }
}
