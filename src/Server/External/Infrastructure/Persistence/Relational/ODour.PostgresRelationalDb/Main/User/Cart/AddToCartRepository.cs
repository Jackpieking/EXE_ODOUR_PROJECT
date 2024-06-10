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

    public Task<bool> IsInputValidQueryAsync(string productId, int quantity, CancellationToken ct)
    {
        return _context
            .Value.Set<ProductEntity>()
            .AnyAsync(
                predicate: product =>
                    product.Id.Equals(productId) && product.QuantityInStock >= quantity,
                cancellationToken: ct
            );
    }

    public Task<ProductEntity> FindProductQueryAsync(string productId, CancellationToken ct)
    {
        return _context
            .Value.Set<ProductEntity>()
            .AsNoTracking()
            .Where(predicate: product => product.Id.Equals(productId))
            .Select(selector: product => new ProductEntity
            {
                Name = product.Name,
                ProductMedias = product.ProductMedias.Select(media => new ProductMediaEntity
                {
                    StorageUrl = media.StorageUrl
                }),
                UnitPrice = product.UnitPrice
            })
            .FirstOrDefaultAsync(cancellationToken: ct);
    }

    public Task<UserTokenEntity> GetRefreshTokenQueryAsync(
        string refreshTokenId,
        CancellationToken ct
    )
    {
        return _context
            .Value.Set<UserTokenEntity>()
            .AsNoTracking()
            .Where(predicate: token => token.LoginProvider.Equals(refreshTokenId))
            .Select(token => new UserTokenEntity
            {
                Value = token.Value,
                ExpiredAt = token.ExpiredAt
            })
            .FirstOrDefaultAsync(cancellationToken: ct);
    }

    public Task<bool> IsUserBannedQueryAsync(Guid userId, CancellationToken ct)
    {
        return _context
            .Value.Set<UserDetailEntity>()
            .AnyAsync(
                predicate: user =>
                    user.UserId == userId
                    && user.AccountStatus.Name.Equals("Bị cấm trong hệ thống"),
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
                                update.SetProperty(
                                    entity => entity.Quantity,
                                    entity => entity.Quantity + newQuantity
                                ),
                            cancellationToken: ct
                        );

                    await _context
                        .Value.Set<ProductEntity>()
                        .Where(predicate: product => product.Id.Equals(productId))
                        .ExecuteUpdateAsync(
                            setPropertyCalls: update =>
                                update.SetProperty(
                                    entity => entity.QuantityInStock,
                                    entity => entity.QuantityInStock - newQuantity
                                ),
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
}
