using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ODour.Domain.Feature.Main.Repository.Auth;
using ODour.Domain.Share.AccountStatus.Entities;
using ODour.Domain.Share.System.Entities;

namespace ODour.PostgresRelationalDb.Main.Auth;

internal sealed class RegisterAsAdminRepository : IRegisterAsAdminRepository
{
    private readonly Lazy<DbContext> _context;

    internal RegisterAsAdminRepository(Lazy<DbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAndAddUserToRoleCommandAsync(
        SystemAccountEntity newUser,
        string password,
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

    public Task<AccountStatusEntity> GetPendingConfirmedStatusQueryAsync(CancellationToken ct)
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
