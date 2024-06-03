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
