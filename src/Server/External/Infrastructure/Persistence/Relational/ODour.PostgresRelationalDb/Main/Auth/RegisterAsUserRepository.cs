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

internal sealed class RegisterAsUserRepository : IRegisterAsUserRepository
{
    private readonly Lazy<DbContext> _context;

    internal RegisterAsUserRepository(Lazy<DbContext> context)
    {
        _context = context;
    }

    public async Task<bool> CreateAndAddUserToRoleCommandAsync(
        UserEntity newUser,
        string password,
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
                    var result = await userManager.CreateAsync(user: newUser, password: password);

                    if (!result.Succeeded)
                    {
                        throw new DbUpdateConcurrencyException();
                    }

                    result = await userManager.AddToRoleAsync(user: newUser, role: "user");

                    if (!result.Succeeded)
                    {
                        throw new DbUpdateConcurrencyException();
                    }

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
            .Value.Set<UserEntity>()
            .AnyAsync(predicate: user => user.NormalizedEmail.Equals(email), cancellationToken: ct);
    }

    public Task RemoveUserCommandAsync(UserEntity newUser, UserManager<UserEntity> userManager)
    {
        return userManager.DeleteAsync(user: newUser);
    }
}
