using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ODour.Domain.Feature.Main.Repository.User.Cart;
using ODour.Domain.Share.Cart.Entities;
using ODour.Domain.Share.Product.Entities;
using ODour.Domain.Share.User.Entities;

namespace ODour.PostgresRelationalDb.Main.User.Cart;

internal sealed class GetCartDetailRepository : IGetCartDetailRepository
{
    private readonly Lazy<DbContext> _context;

    internal GetCartDetailRepository(Lazy<DbContext> context)
    {
        _context = context;
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
                    ProductMedias = entity.Product.ProductMedias.Select(
                        media => new ProductMediaEntity { StorageUrl = media.StorageUrl }
                    )
                }
            })
            .ToListAsync(cancellationToken: ct);
    }
}
