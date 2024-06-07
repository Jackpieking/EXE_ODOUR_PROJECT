using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;

namespace ODour.Application.Share.BackgroundJob;

public interface IQueueHandler
{
    Task QueueAsync(ICommand backgroundJobCommand, CancellationToken ct);
}
