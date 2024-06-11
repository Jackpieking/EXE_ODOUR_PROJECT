using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;

namespace ODour.Application.Share.BackgroundJob;

public interface IQueueHandler
{
    Task QueueAsync(
        ICommand backgroundJobCommand,
        DateTime? executeAfter,
        DateTime? expireOn,
        CancellationToken ct
    );
}
