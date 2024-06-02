using System;
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

    public Task<bool> IsRefreshTokenFoundQueryAsync(
        string refreshToken,
        string refreshTokenId,
        CancellationToken ct
    )
    {
        return _context
            .Value.Set<UserTokenEntity>()
            .AnyAsync(
                predicate: token =>
                    token.LoginProvider.Equals(refreshTokenId) && token.Value.Equals(refreshToken),
                cancellationToken: ct
            );
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
