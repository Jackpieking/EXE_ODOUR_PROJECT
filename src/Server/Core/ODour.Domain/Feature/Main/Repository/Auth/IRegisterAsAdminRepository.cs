using System.Threading;
using System.Threading.Tasks;
using ODour.Domain.Share.AccountStatus.Entities;
using ODour.Domain.Share.System.Entities;
using ODour.Domain.Share.User.Entities;

namespace ODour.Domain.Feature.Main.Repository.Auth;

public interface IRegisterAsAdminRepository
{
    #region Quey
    Task<bool> IsUserFoundByNormalizedEmailQueryAsync(string email, CancellationToken ct);

    Task<AccountStatusEntity> GetPendingConfirmedStatusQueryAsync(CancellationToken ct);
    #endregion

    #region Command
    Task<bool> CreateAndAddUserToRoleCommandAsync(
        SystemAccountEntity newUser,
        string password,
        CancellationToken ct
    );
    #endregion
}
