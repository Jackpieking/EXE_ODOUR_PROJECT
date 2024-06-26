using System;
using System.Threading;
using System.Threading.Tasks;

namespace ODour.Domain.Feature.Main.Repository.User.Cart;

public interface IRemoveFromCartRepository
{
    #region Query
    Task<bool> IsRefreshTokenFoundQueryAsync(string refreshTokenId, CancellationToken ct);

    Task<bool> IsInputValidQueryAsync(string productId, int quantity, CancellationToken ct);

    Task<bool> UpdateQuantityQueryAsync(
        string productId,
        int newQuantity,
        Guid userId,
        CancellationToken ct
    );
    #endregion
}
