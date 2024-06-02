using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ODour.Domain.Feature.Main.Repository.Auth;
using ODour.Domain.Share.User.Entities;

namespace ODour.PostgresRelationalDb.Main.Auth;

internal sealed class ForgotPasswordRepository : IForgotPasswordRepository
{
    private readonly Lazy<DbContext> _context;

    internal ForgotPasswordRepository(Lazy<DbContext> context)
    {
        _context = context;
    }

    public async Task<bool> AddUserPasswordChangingTokenCommandAsync(
        IEnumerable<UserTokenEntity> userTokenEntities,
        CancellationToken ct
    )
    {
        {
            try
            {
                await _context
                    .Value.Set<UserTokenEntity>()
                    .AddRangeAsync(entities: userTokenEntities, cancellationToken: ct);

                await _context.Value.SaveChangesAsync(cancellationToken: ct);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }

    public Task<UserEntity> GetUserByEmailQueryAsync(string email, CancellationToken ct)
    {
        email = email.ToUpper();

        return _context
            .Value.Set<UserEntity>()
            .AsNoTracking()
            .Where(predicate: user => user.NormalizedEmail.Equals(email))
            .Select(selector: user => new UserEntity { Id = user.Id })
            .FirstOrDefaultAsync(cancellationToken: ct);
    }

    public Task<bool> IsUserFoundByNormalizedEmailQueryAsync(string email, CancellationToken ct)
    {
        email = email.ToUpper();

        return _context
            .Value.Set<UserEntity>()
            .AnyAsync(predicate: user => user.NormalizedEmail.Equals(email), cancellationToken: ct);
    }

    public Task<bool> IsUserTemporarilyRemovedQueryAsync(string email, CancellationToken ct)
    {
        email = email.ToUpper();

        return _context
            .Value.Set<UserEntity>()
            .AnyAsync(
                predicate: user =>
                    user.NormalizedEmail.Equals(email) && user.UserDetail.IsTemporarilyRemoved,
                cancellationToken: ct
            );
    }
}
