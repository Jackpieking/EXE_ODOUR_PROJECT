using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ODour.Domain.Feature.Main.Repository.User.Cart;
using ODour.Domain.Share.Cart.Entities;
using ODour.Domain.Share.Product.Entities;
using ODour.Domain.Share.User.Entities;

namespace ODour.PostgresRelationalDb.Main.User.Cart;

internal sealed class AddToCartRepository : IAddToCartRepository
{
    private readonly Lazy<DbContext> _context;

    internal AddToCartRepository(Lazy<DbContext> context)
    {
        _context = context;
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

    public async Task<bool> AddItemToCartQueryAsync(CartItemEntity cartItem, CancellationToken ct)
    {
        try
        {
            await _context
                .Value.Set<CartItemEntity>()
                .AddAsync(entity: cartItem, cancellationToken: ct);

            await _context.Value.SaveChangesAsync(cancellationToken: ct);
        }
        catch
        {
            return false;
        }

        return true;
    }

    public Task<bool> IsCartItemFoundQueryAsync(string productId, Guid userId, CancellationToken ct)
    {
        return _context
            .Value.Set<CartItemEntity>()
            .AnyAsync(
                predicate: cartItem =>
                    cartItem.ProductId.Equals(productId) && cartItem.UserId == userId,
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
                    await _context
                        .Value.Set<CartItemEntity>()
                        .Where(predicate: cartItem =>
                            cartItem.ProductId.Equals(productId) && cartItem.UserId == userId
                        )
                        .ExecuteUpdateAsync(
                            setPropertyCalls: update =>
                                update.SetProperty(entity => entity.Quantity, newQuantity),
                            cancellationToken: ct
                        );

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

    public Task<CartItemEntity> FindCartItemQueryAsync(
        string productId,
        Guid userId,
        CancellationToken ct
    )
    {
        return _context
            .Value.Set<CartItemEntity>()
            .AsNoTracking()
            .Where(predicate: cartItem =>
                cartItem.ProductId.Equals(productId) && cartItem.UserId == userId
            )
            .Select(selector: cartItem => new CartItemEntity { Quantity = cartItem.Quantity })
            .FirstOrDefaultAsync(cancellationToken: ct);
    }

    public Task<ProductEntity> FindProductQueryAsync(string productId, CancellationToken ct)
    {
        return _context
            .Value.Set<ProductEntity>()
            .AsNoTracking()
            .Where(predicate: product => product.Id.Equals(productId))
            .Select(selector: product => new ProductEntity
            {
                QuantityInStock = product.QuantityInStock
            })
            .FirstOrDefaultAsync(cancellationToken: ct);
    }

    public Task<int> GetTotalNumberOfCartItemQueryAsync(Guid userId, CancellationToken ct)
    {
        return _context
            .Value.Set<CartItemEntity>()
            .CountAsync(cartItem => cartItem.UserId == userId, cancellationToken: ct);
    }
}
