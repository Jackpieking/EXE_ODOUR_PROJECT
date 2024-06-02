using System;
using Microsoft.Extensions.DependencyInjection;

namespace ODour.Application.Share.Common;

public static class CommonMethod
{
    public static IServiceCollection MakeSingletonLazy<T>(this IServiceCollection services)
        where T : class
    {
        return services.AddSingleton<Lazy<T>>(implementationFactory: provider =>
            new(valueFactory: () => provider.GetRequiredService<T>())
        );
    }

    public static IServiceCollection MakeScopedLazy<T>(this IServiceCollection services)
        where T : class
    {
        return services.AddScoped<Lazy<T>>(implementationFactory: provider =>
            new(valueFactory: () => provider.GetRequiredService<T>())
        );
    }
}
