using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ODour.Domain.Share.AccountStatus.Entities;
using ODour.Domain.Share.Role.Entities;
using ODour.Domain.Share.System.Entities;

namespace ODour.Domain.Feature.Main.Repository.Auth;

public interface IRegisterAsAdminRepository
{
    #region Query
    Task<bool> IsUserFoundByNormalizedEmailQueryAsync(string email, CancellationToken ct);

    Task<AccountStatusEntity> GetPendingConfirmedAccountStatusQueryAsync(CancellationToken ct);

    Task<RoleEntity> GetAdminRoleQueryAsync(CancellationToken ct);
    #endregion

    #region Command
    Task<bool> CreateAdminCommandAsync(
        SystemAccountEntity newAdmin,
        List<SystemAccountRoleEntity> adminRoles,
        List<SystemAccountTokenEntity> adminTokens,
        CancellationToken ct
    );
    #endregion
}
