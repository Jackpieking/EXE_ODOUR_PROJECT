using Microsoft.Extensions.DependencyInjection;
using ODour.AppBackgroundJob.Handler;
using ODour.Application.Share.BackgroundJob;
using ODour.Application.Share.Common;

namespace ODour.AppBackgroundJob.ServiceConfigs;

/// <summary>
///     Core service config.
/// </summary>
internal static class CoreServiceConfig
{
    internal static void Config(IServiceCollection services)
    {
        services.AddSingleton<IJobHandler, AppJobHandler>().MakeSingletonLazy<IJobHandler>();
    }
}
