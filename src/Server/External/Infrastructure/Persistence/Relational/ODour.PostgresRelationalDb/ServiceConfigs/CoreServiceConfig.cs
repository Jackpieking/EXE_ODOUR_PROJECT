using Microsoft.Extensions.DependencyInjection;
using ODour.Application.Share.Common;
using ODour.PostgresRelationalDb.Data;

namespace ODour.PostgresRelationalDb.ServiceConfigs;

internal static class CoreServiceConfig
{
    internal static void Config(IServiceCollection services)
    {
        services.MakeScopedLazy<ODourContext>();
    }
}
