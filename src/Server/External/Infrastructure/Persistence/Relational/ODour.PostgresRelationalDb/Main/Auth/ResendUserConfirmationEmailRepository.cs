using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ODour.Domain.Feature.Main.Repository.Auth;
using ODour.Domain.Share.User.Entities;

namespace ODour.PostgresRelationalDb.Main.Auth;

internal sealed class ResendUserConfirmationEmailRepository : IResendUserConfirmationEmailRepository
{
    private readonly Lazy<DbContext> _context;

    internal ResendUserConfirmationEmailRepository(Lazy<DbContext> context)
    {
        _context = context;
    }

    public async Task<bool> AddUserConfirmedEmailTokenCommandAsync(
        UserTokenEntity userTokenEntity,
        CancellationToken ct
    )
    {
        try
        {
            await _context
                .Value.Set<UserTokenEntity>()
                .AddAsync(entity: userTokenEntity, cancellationToken: ct);

            await _context.Value.SaveChangesAsync(cancellationToken: ct);
        }
        catch
        {
            return false;
        }

        return true;
    }

    public Task<bool> HasUserConfirmedEmailQueryAsync(string email, CancellationToken ct)
    {
        email = email.ToUpper();

        return _context
            .Value.Set<UserEntity>()
            .AnyAsync(
                predicate: user => user.NormalizedEmail.Equals(email) && user.EmailConfirmed,
                cancellationToken: ct
            );
    }

    public Task<bool> IsUserFoundByNormalizedEmailQueryAsync(string email, CancellationToken ct)
    {
        email = email.ToUpper();

        return _context
            .Value.Set<UserEntity>()
            .AnyAsync(predicate: user => user.NormalizedEmail.Equals(email), cancellationToken: ct);
    }

    public Task<bool> IsUserBannedQueryAsync(string email, CancellationToken ct)
    {
        email = email.ToUpper();

        return _context
            .Value.Set<UserEntity>()
            .AnyAsync(
                predicate: user =>
                    user.NormalizedEmail.Equals(email)
                    && user.UserDetail.AccountStatus.Name.Equals("Bị cấm trong hệ thống"),
                cancellationToken: ct
            );
    }
}
