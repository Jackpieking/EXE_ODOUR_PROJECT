using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ODour.Domain.Share.AccountStatus.Entities;
using ODour.Domain.Share.User.Entities;

namespace ODour.Domain.Feature.Main.Repository.Auth
{
    public interface IRegisterAsUserRepository
    {
        #region Quey
        Task<bool> IsUserFoundByNormalizedEmailQueryAsync(string email, CancellationToken ct);

        Task<AccountStatusEntity> GetPendingConfirmedStatusQueryAsync(CancellationToken ct);
        #endregion

        #region Command
        Task<bool> CreateAndAddUserToRoleCommandAsync(
            UserEntity newUser,
            string password,
            UserManager<UserEntity> userManager,
            CancellationToken ct
        );

        Task RemoveUserCommandAsync(UserEntity newUser, UserManager<UserEntity> userManager);
        #endregion
    }
}