using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ODour.Application.Share.BackgroundJob;

namespace ODour.AppBackgroundJob.Handler;

public sealed class FastEndpointQueueHandler : IQueueHandler
{
    public Task QueueAsync(
        ICommand backgroundJobCommand,
        DateTime? executeAfter,
        DateTime? expireOn,
        CancellationToken ct
    )
    {
        return backgroundJobCommand.QueueJobAsync(
            executeAfter: executeAfter,
            expireOn: expireOn,
            ct: ct
        );
    }
}
