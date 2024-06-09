using FastEndpoints;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ODour.AppBackgroundJob.Share;
using ODour.Domain.Share.System.Entities;

namespace ODour.AppBackgroundJob;

public static class AppBackgroundJobServiceConfig
{
    public static IServiceCollection Config(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // ====
        services.AddJobQueues<JobRecordEntity, FastEndpointJobStorageProvider>();

        return services;
    }
}
