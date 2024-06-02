using Microsoft.Extensions.DependencyInjection;

namespace ODour.FastEndpointApi.ServiceConfigs
{
    internal static class DataProtectionServiceConfig
    {
        internal static void Config(IServiceCollection services)
        {
            services.AddDataProtection();
        }
    }
}
