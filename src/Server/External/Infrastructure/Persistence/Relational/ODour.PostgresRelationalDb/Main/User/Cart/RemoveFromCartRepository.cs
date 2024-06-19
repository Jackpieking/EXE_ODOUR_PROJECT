using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ODour.Domain.Feature.Main.Repository.User.Cart;
using ODour.Domain.Share.Cart.Entities;
using ODour.Domain.Share.User.Entities;

namespace ODour.PostgresRelationalDb.Main.User.Cart;

internal sealed class RemoveFromCartRepository : IRemoveFromCartRepository
{
    private readonly Lazy<DbContext> _context;

    internal RemoveFromCartRepository(Lazy<DbContext> context)
    {
        _context = context;
    }

    public Task<bool> IsInputValidQueryAsync(string productId, int quantity, CancellationToken ct)
    {
        return _context
            .Value.Set<CartItemEntity>()
            .AnyAsync(
                predicate: cartItem =>
                    cartItem.ProductId.Equals(productId) && cartItem.Quantity - quantity >= 0,
                cancellationToken: ct
            );
    }

    public Task<bool> IsRefreshTokenFoundQueryAsync(string refreshTokenId, CancellationToken ct)
    {
        return _context
            .Value.Set<UserTokenEntity>()
            .AsNoTracking()
            .AnyAsync(
                predicate: token =>
                    token.LoginProvider.Equals(refreshTokenId) && token.ExpiredAt > DateTime.UtcNow,
                cancellationToken: ct
            );
    }

    public async Task<bool> UpdateQuantityQueryAsync(
        string productId,
        int newQuantity,
        Guid userId,
        CancellationToken ct
    )
    {
        var dbResult = false;

        await _context
            .Value.Database.CreateExecutionStrategy()
            .ExecuteAsync(operation: async () =>
            {
                await using var dbTransaction = await _context.Value.Database.BeginTransactionAsync(
                    cancellationToken: ct
                );

                try
                {
                    var foundCartItem = await _context
                        .Value.Set<CartItemEntity>()
                        .Where(predicate: cartItem =>
                            cartItem.ProductId.Equals(productId) && cartItem.UserId == userId
                        )
                        .Select(selector: cartItem => new CartItemEntity
                        {
                            Quantity = cartItem.Quantity
                        })
                        .FirstOrDefaultAsync(cancellationToken: ct);

                    // calculate remain quantity
                    var remainQuantity = foundCartItem.Quantity - newQuantity;

                    // Remain quantity is 0, then remove from cart
                    if (remainQuantity == default)
                    {
                        await _context
                            .Value.Set<CartItemEntity>()
                            .Where(predicate: cartItem =>
                                cartItem.ProductId.Equals(productId) && cartItem.UserId == userId
                            )
                            .ExecuteDeleteAsync(cancellationToken: ct);
                    }
                    else
                    {
                        await _context
                            .Value.Set<CartItemEntity>()
                            .Where(predicate: cartItem =>
                                cartItem.ProductId.Equals(productId) && cartItem.UserId == userId
                            )
                            .ExecuteUpdateAsync(
                                setPropertyCalls: update =>
                                    update.SetProperty(entity => entity.Quantity, remainQuantity),
                                cancellationToken: ct
                            );
                    }

                    await dbTransaction.CommitAsync(cancellationToken: ct);

                    dbResult = true;
                }
                catch
                {
                    await dbTransaction.RollbackAsync(cancellationToken: ct);
                }
            });

        return dbResult;
    }
}
