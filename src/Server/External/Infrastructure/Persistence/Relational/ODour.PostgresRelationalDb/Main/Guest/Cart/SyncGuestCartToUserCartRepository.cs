using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ODour.Domain.Feature.Main.Repository.Guest.Cart;
using ODour.Domain.Share.Cart.Entities;
using ODour.Domain.Share.User.Entities;

namespace ODour.PostgresRelationalDb.Main.Guest.Cart;

internal sealed class SyncGuestCartToUserCartRepository : ISyncGuestCartToUserCartRepository
{
    private readonly Lazy<DbContext> _context;

    internal SyncGuestCartToUserCartRepository(Lazy<DbContext> context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CartItemEntity>> FindCartByUserIdQueryAsync(
        Guid userId,
        CancellationToken ct
    )
    {
        return await _context
            .Value.Set<CartItemEntity>()
            .AsNoTracking()
            .Where(predicate: cartItem => cartItem.UserId == userId)
            .Select(selector: cartItem => new CartItemEntity
            {
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity,
                Product = new() { QuantityInStock = cartItem.Product.QuantityInStock }
            })
            .ToListAsync(cancellationToken: ct);
    }

    public Task<bool> IsRefreshTokenFoundQueryAsync(string refreshTokenId, CancellationToken ct)
    {
        return _context
            .Value.Set<UserTokenEntity>()
            .AnyAsync(
                predicate: token =>
                    token.LoginProvider.Equals(refreshTokenId) && token.ExpiredAt > DateTime.UtcNow,
                cancellationToken: ct
            );
    }

    public async Task<bool> UpdateUserCartCommandAsync(
        IEnumerable<CartItemEntity> newUserCartItems,
        IEnumerable<CartItemEntity> currentUserCartItems,
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
                    // Add new user cart items
                    await _context
                        .Value.Set<CartItemEntity>()
                        .AddRangeAsync(entities: newUserCartItems, cancellationToken: ct);

                    // Update current user cart items
                    foreach (var currentUserCartItem in currentUserCartItems)
                    {
                        await _context
                            .Value.Set<CartItemEntity>()
                            .Where(cartItem =>
                                cartItem.UserId == userId
                                && cartItem.ProductId.Equals(currentUserCartItem.ProductId)
                                && cartItem.Quantity != currentUserCartItem.Quantity
                            )
                            .ExecuteUpdateAsync(setPropertyCalls: builder =>
                                builder.SetProperty(
                                    cartItem => cartItem.Quantity,
                                    currentUserCartItem.Quantity
                                )
                            );
                    }

                    await _context.Value.SaveChangesAsync(cancellationToken: ct);

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
