using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ODour.Domain.Feature.Main.Repository.User.Cart;
using ODour.Domain.Share.Cart.Entities;
using ODour.Domain.Share.User.Entities;

namespace ODour.PostgresRelationalDb.Main.User.Cart;

internal sealed class GetCartDetailRepository : IGetCartDetailRepository
{
    private readonly Lazy<DbContext> _context;

    internal GetCartDetailRepository(Lazy<DbContext> context)
    {
        _context = context;
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

    public async Task<IEnumerable<CartItemEntity>> GetCartItemsOfUserQueryAsync(
        Guid userId,
        CancellationToken ct
    )
    {
        return await _context
            .Value.Set<CartItemEntity>()
            .AsNoTracking()
            .Where(predicate: entity => entity.UserId == userId)
            .Select(selector: entity => new CartItemEntity
            {
                Quantity = entity.Quantity,
                ProductId = entity.ProductId,
                Product = new()
                {
                    Name = entity.Product.Name,
                    UnitPrice = entity.Product.UnitPrice,
                }
            })
            .ToListAsync(cancellationToken: ct);
    }
}
