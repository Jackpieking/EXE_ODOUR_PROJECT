using System.Threading;
using System.Threading.Tasks;

namespace ODour.Domain.Feature.Main.Repository.Auth;

public interface ILogoutRepository
{
    #region Quries
    Task<bool> IsRefreshTokenFoundQueryAsync(string refreshTokenId, CancellationToken ct);
    #endregion

    #region Commands
    Task<bool> RemoveRefreshTokenCommandAsync(string refreshTokenId, CancellationToken ct);
    #endregion
}
