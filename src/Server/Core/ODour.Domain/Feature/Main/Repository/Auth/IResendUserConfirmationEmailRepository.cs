using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ODour.Domain.Share.User.Entities;

namespace ODour.Domain.Feature.Main.Repository.Auth;

public interface IResendUserConfirmationEmailRepository
{
    #region  Query
    Task<bool> IsUserFoundByNormalizedEmailQueryAsync(string email, CancellationToken ct);

    Task<bool> IsUserTemporarilyRemovedQueryAsync(string email, CancellationToken ct);

    Task<bool> HasUserConfirmedEmailQueryAsync(string email, CancellationToken ct);
    #endregion

    #region Command
    Task<bool> AddUserPasswordChangingTokenCommandAsync(
        IEnumerable<UserTokenEntity> userTokenEntities,
        CancellationToken ct
    );
    #endregion
}
