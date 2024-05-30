using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ODour.Domain.Share.System.Entities;

namespace ODour.AppBackgroundJob.Handler;

internal sealed class DefaultJobStorageProvider : IJobStorageProvider<JobRecordEntity>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public DefaultJobStorageProvider(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<IEnumerable<JobRecordEntity>> GetNextBatchAsync(
        PendingJobSearchParams<JobRecordEntity> parameters
    )
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.TryResolve<Lazy<DbContext>>();

        return await context
            .Value.Set<JobRecordEntity>()
            .Where(predicate: parameters.Match)
            .Take(count: parameters.Limit)
            .ToListAsync(cancellationToken: parameters.CancellationToken);
    }

    public async Task MarkJobAsCompleteAsync(JobRecordEntity r, CancellationToken ct)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.TryResolve<Lazy<DbContext>>();

        context.Value.Update(entity: r);

        await context.Value.SaveChangesAsync(cancellationToken: ct);
    }

    public async Task OnHandlerExecutionFailureAsync(
        JobRecordEntity r,
        Exception exception,
        CancellationToken ct
    )
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.TryResolve<Lazy<DbContext>>();

        r.ExecuteAfter = DateTime.UtcNow.AddMinutes(value: 1);

        context.Value.Update(entity: r);

        await context.Value.SaveChangesAsync(cancellationToken: ct);
    }

    public async Task PurgeStaleJobsAsync(StaleJobSearchParams<JobRecordEntity> parameters)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.TryResolve<Lazy<DbContext>>();

        var staleJobs = context.Value.Set<JobRecordEntity>().Where(predicate: parameters.Match);

        context.Value.RemoveRange(entities: staleJobs);

        await context.Value.SaveChangesAsync(cancellationToken: parameters.CancellationToken);
    }

    public async Task StoreJobAsync(JobRecordEntity r, CancellationToken ct)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.TryResolve<Lazy<DbContext>>();

        await context.Value.AddAsync(entity: r, cancellationToken: ct);

        await context.Value.SaveChangesAsync(cancellationToken: ct);
    }
}
