using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ODour.Domain.Feature.Main.Repository.User.Product;
using ODour.Domain.Share.Category.Entities;
using ODour.Domain.Share.Filter;
using ODour.Domain.Share.Product.Entities;
using static ODour.Application.Share.Common.CommonConstant;

namespace ODour.PostgresRelationalDb.Main.User.Product;

internal sealed class GetAllProductsRepository : IGetAllProductsRepository
{
    private readonly Lazy<DbContext> _context;

    public GetAllProductsRepository(Lazy<DbContext> context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProductEntity>> GetAllProductsQueryAsync(
        int currentPage,
        int pageSize,
        Guid categoryId,
        string sortType,
        CancellationToken ct
    )
    {
        var baseQuery = _context.Value.Set<ProductEntity>().AsNoTracking();

        // Apply category filter if needed
        if (categoryId != App.DefaultGuidValue)
        {
            baseQuery = baseQuery.Where(predicate: product => product.Category.Id == categoryId);
        }

        // Apply sorting based on sortType
        baseQuery = sortType switch
        {
            SortingFilterTypes.Product.PROD_NAME_DESC
                => baseQuery.OrderByDescending(keySelector: product => product.Name),
            SortingFilterTypes.Product.PROD_NAME_ASC
                => baseQuery.OrderBy(keySelector: product => product.Name),
            SortingFilterTypes.Product.PROD_PRICE_ASC
                => baseQuery.OrderBy(keySelector: product => product.UnitPrice),
            SortingFilterTypes.Product.PROD_PRICE_DESC
                => baseQuery.OrderByDescending(keySelector: product => product.UnitPrice),
            _ => baseQuery
        };

        // Apply pagination
        var pagedQuery = baseQuery.Skip(count: (currentPage - 1) * pageSize).Take(count: pageSize);

        // Project the final result
        var result = await pagedQuery
            .Select(selector: product => new ProductEntity
            {
                Id = product.Id,
                Name = product.Name,
                UnitPrice = product.UnitPrice,
                ProductStatus = new() { Name = product.ProductStatus.Name },
                ProductMedias = product
                    .ProductMedias.Select(image => new ProductMediaEntity
                    {
                        StorageUrl = image.StorageUrl
                    })
                    .ToList(),
                Category = new() { Id = product.Category.Id, Name = product.Category.Name }
            })
            .ToListAsync(cancellationToken: ct);

        return result;
    }

    public Task<int> GetProductsCountQueryAsync(Guid categoryId, CancellationToken ct)
    {
        if (categoryId != App.DefaultGuidValue)
        {
            return _context
                .Value.Set<ProductEntity>()
                .CountAsync(
                    predicate: product => product.Category.Id == categoryId,
                    cancellationToken: ct
                );
        }

        return _context.Value.Set<ProductEntity>().CountAsync(cancellationToken: ct);
    }

    public Task<bool> IsCategoryFoundQueryAsync(Guid categoryId, CancellationToken ct)
    {
        return _context
            .Value.Set<CategoryEntity>()
            .AnyAsync(predicate: category => category.Id == categoryId, cancellationToken: ct);
    }

    public bool IsSortFilterFoundQuery(string sortType)
    {
        return SortingFilterTypes.Product.AppFilterList.Any(predicate: filter =>
            filter.Equals(value: sortType)
        );
    }
}
