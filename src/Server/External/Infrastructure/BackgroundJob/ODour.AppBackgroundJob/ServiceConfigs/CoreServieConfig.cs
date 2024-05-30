using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using ODour.AppBackgroundJob.Handler;
using ODour.Domain.Share.System.Entities;

namespace ODour.AppBackgroundJob.ServiceConfigs;

/// <summary>
///     Core service config.
/// </summary>
internal static class CoreServiceConfig
{
    internal static void Config(IServiceCollection services)
    {
        services.AddJobQueues<JobRecordEntity, DefaultJobStorageProvider>();
    }
}
