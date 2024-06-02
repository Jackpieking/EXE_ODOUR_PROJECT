using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ODour.Domain.Feature.Main.Repository.Auth;
using ODour.Domain.Share.AccountStatus.Entities;
using ODour.Domain.Share.Role.Entities;
using ODour.Domain.Share.System.Entities;

namespace ODour.PostgresRelationalDb.Main.Auth;

internal sealed class RegisterAsAdminRepository : IRegisterAsAdminRepository
{
    private readonly Lazy<DbContext> _context;

    internal RegisterAsAdminRepository(Lazy<DbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAdminCommandAsync(
        SystemAccountEntity newAdmin,
        List<SystemAccountRoleEntity> adminRoles,
        List<SystemAccountTokenEntity> adminTokens,
        CancellationToken ct
    )
    {
        try
        {
            await _context
                .Value.Set<SystemAccountRoleEntity>()
                .AddRangeAsync(entities: adminRoles, cancellationToken: ct);

            await _context
                .Value.Set<SystemAccountTokenEntity>()
                .AddRangeAsync(entities: adminTokens, cancellationToken: ct);

            await _context
                .Value.Set<SystemAccountEntity>()
                .AddAsync(entity: newAdmin, cancellationToken: ct);

            await _context.Value.SaveChangesAsync(cancellationToken: ct);
        }
        catch
        {
            return false;
        }

        return true;
    }

    public Task<RoleEntity> GetAdminRoleQueryAsync(CancellationToken ct)
    {
        return _context
            .Value.Set<RoleEntity>()
            .AsNoTracking()
            .Where(predicate: entity => entity.Name.Equals("admin"))
            .Select(selector: entity => new RoleEntity { Id = entity.Id })
            .FirstOrDefaultAsync(cancellationToken: ct);
    }

    public Task<AccountStatusEntity> GetPendingConfirmedAccountStatusQueryAsync(
        CancellationToken ct
    )
    {
        return _context
            .Value.Set<AccountStatusEntity>()
            .AsNoTracking()
            .Where(predicate: entity => entity.Name.Equals("Chờ xác nhận đăng ký"))
            .Select(selector: entity => new AccountStatusEntity { Id = entity.Id, })
            .FirstOrDefaultAsync(cancellationToken: ct);
    }

    public Task<bool> IsUserFoundByNormalizedEmailQueryAsync(string email, CancellationToken ct)
    {
        email = email.ToUpper();

        return _context
            .Value.Set<SystemAccountEntity>()
            .AnyAsync(predicate: user => user.NormalizedEmail.Equals(email), cancellationToken: ct);
    }
}
