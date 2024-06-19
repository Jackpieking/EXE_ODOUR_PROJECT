using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ODour.Application.Share.BackgroundJob;
using ODour.Domain.Share.System.Entities;
using static ODour.Application.Share.Common.CommonConstant;

namespace ODour.AppBackgroundJob.Share;

public sealed class FastEndpointJobStorageProvider : IJobStorageProvider<JobRecordEntity>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public FastEndpointJobStorageProvider(IServiceScopeFactory scopeFactory)
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
            .AsNoTracking()
            .Where(predicate: parameters.Match)
            .Take(count: parameters.Limit)
            .ToListAsync(cancellationToken: parameters.CancellationToken);
    }

    public async Task MarkJobAsCompleteAsync(JobRecordEntity r, CancellationToken ct)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.TryResolve<Lazy<DbContext>>();

        await context
            .Value.Database.CreateExecutionStrategy()
            .ExecuteAsync(operation: async () =>
            {
                await using var dbTransaction = await context.Value.Database.BeginTransactionAsync(
                    cancellationToken: ct
                );

                try
                {
                    var failureReasonsAsJson = JsonSerializer.Serialize(value: string.Empty);

                    //if this is a recurring job command, reschedule the next execution like so:
                    if (r.Command is IRecurringJob cronJob)
                    {
                        await context
                            .Value.Set<JobRecordEntity>()
                            .Where(predicate: entity => entity.Id == r.Id)
                            .ExecuteUpdateAsync(
                                setPropertyCalls: builder =>
                                    builder
                                        .SetProperty(
                                            entity => entity.ExecuteAfter,
                                            DateTime.UtcNow.Add(cronJob.Frequency)
                                        )
                                        .SetProperty(
                                            entity => entity.FailureReason,
                                            failureReasonsAsJson
                                        ),
                                cancellationToken: ct
                            );
                    }
                    else
                    {
                        await context
                            .Value.Set<JobRecordEntity>()
                            .Where(predicate: entity => entity.Id == r.Id)
                            .ExecuteUpdateAsync(
                                setPropertyCalls: builder =>
                                    builder
                                        .SetProperty(entity => entity.IsComplete, true)
                                        .SetProperty(
                                            entity => entity.FailureReason,
                                            failureReasonsAsJson
                                        ),
                                cancellationToken: ct
                            );
                    }

                    await dbTransaction.CommitAsync(cancellationToken: ct);
                }
                catch
                {
                    await dbTransaction.RollbackAsync(cancellationToken: ct);
                }
            });
    }

    public async Task OnHandlerExecutionFailureAsync(
        JobRecordEntity r,
        Exception exception,
        CancellationToken ct
    )
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.TryResolve<Lazy<DbContext>>();

        const int MaxRetryCount = 3;

        await context
            .Value.Database.CreateExecutionStrategy()
            .ExecuteAsync(operation: async () =>
            {
                await using var dbTransaction = await context.Value.Database.BeginTransactionAsync(
                    cancellationToken: ct
                );

                try
                {
                    if (r.FailureCount == MaxRetryCount)
                    {
                        var failureReasonseAsJson = JsonSerializer.Serialize(
                            value: exception.Message
                        );

                        await context
                            .Value.Set<JobRecordEntity>()
                            .Where(predicate: entity => entity.Id == r.Id)
                            .ExecuteUpdateAsync(
                                setPropertyCalls: builder =>
                                    builder
                                        .SetProperty(entity => entity.IsComplete, true)
                                        .SetProperty(entity => entity.CancelledOn, DateTime.UtcNow)
                                        .SetProperty(
                                            entity => entity.FailureReason,
                                            failureReasonseAsJson
                                        ),
                                cancellationToken: ct
                            );
                    }
                    else if (r.FailureCount < MaxRetryCount)
                    {
                        var failureReasonsAsJson = JsonSerializer.Serialize(
                            value: exception.Message
                        );

                        if (r.Command is IRecurringJob cronJob)
                        {
                            await context
                                .Value.Set<JobRecordEntity>()
                                .Where(predicate: entity => entity.Id == r.Id)
                                .ExecuteUpdateAsync(
                                    setPropertyCalls: builder =>
                                        builder
                                            .SetProperty(
                                                entity => entity.ExecuteAfter,
                                                DateTime.UtcNow.Add(cronJob.Frequency)
                                            )
                                            .SetProperty(
                                                entity => entity.FailureCount,
                                                r.FailureCount + 1
                                            )
                                            .SetProperty(
                                                entity => entity.FailureReason,
                                                failureReasonsAsJson
                                            ),
                                    cancellationToken: ct
                                );
                        }
                        else
                        {
                            await context
                                .Value.Set<JobRecordEntity>()
                                .Where(predicate: entity => entity.Id == r.Id)
                                .ExecuteUpdateAsync(
                                    setPropertyCalls: builder =>
                                        builder
                                            .SetProperty(
                                                entity => entity.FailureReason,
                                                failureReasonsAsJson
                                            )
                                            .SetProperty(
                                                entity => entity.FailureCount,
                                                r.FailureCount + 1
                                            )
                                            .SetProperty(
                                                entity => entity.ExecuteAfter,
                                                DateTime.UtcNow.AddSeconds(10)
                                            )
                                            .SetProperty(
                                                entity => entity.ExpireOn,
                                                DateTime.UtcNow.AddSeconds(60)
                                            ),
                                    cancellationToken: ct
                                );
                        }
                    }

                    await dbTransaction.CommitAsync(cancellationToken: ct);
                }
                catch
                {
                    await dbTransaction.RollbackAsync(cancellationToken: ct);
                }
            });
    }

    public async Task PurgeStaleJobsAsync(StaleJobSearchParams<JobRecordEntity> parameters)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.TryResolve<Lazy<DbContext>>();

        await context
            .Value.Database.CreateExecutionStrategy()
            .ExecuteAsync(operation: async () =>
            {
                await using var dbTransaction = await context.Value.Database.BeginTransactionAsync(
                    cancellationToken: parameters.CancellationToken
                );

                try
                {
                    await context
                        .Value.Set<JobRecordEntity>()
                        .Where(predicate: parameters.Match)
                        .ExecuteDeleteAsync(cancellationToken: parameters.CancellationToken);

                    await dbTransaction.CommitAsync(
                        cancellationToken: parameters.CancellationToken
                    );
                }
                catch
                {
                    await dbTransaction.RollbackAsync(
                        cancellationToken: parameters.CancellationToken
                    );
                }
            });
    }

    public async Task StoreJobAsync(JobRecordEntity r, CancellationToken ct)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.TryResolve<Lazy<DbContext>>();

        r.FailureReason = JsonSerializer.Serialize(value: string.Empty);
        r.CancelledOn = App.MinTimeInUTC;
        r.FailureCount = default;

        if (r.Command is IRecurringJob)
        {
            r.IsComplete = false;
            r.ExpireOn = DateTime.MaxValue.ToUniversalTime();
        }

        await context.Value.Set<JobRecordEntity>().AddAsync(entity: r, cancellationToken: ct);

        await context.Value.SaveChangesAsync(cancellationToken: ct);
    }
}
