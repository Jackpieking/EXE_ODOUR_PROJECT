using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ODour.Configuration.Infrastructure.Persistence.SessionStorage;

namespace ODour.RedisSessionStorage;

public static class RedisSessionStorageServiceConfig
{
    public static void Config(this IServiceCollection services, IConfiguration configuration)
    {
        var redisSessionStorageOption = configuration
            .GetRequiredSection(key: "Session")
            .GetRequiredSection(key: "Redis")
            .Get<RedisSessionStorageOption>();

        // ====
        services.AddSession(configure: options =>
        {
            options.IdleTimeout = TimeSpan.FromSeconds(
                redisSessionStorageOption.IdleTimeoutInSecond
            );
            options.Cookie.Name = redisSessionStorageOption.Cookie.Name;
            options.Cookie.HttpOnly = redisSessionStorageOption.Cookie.HttpOnly;
            options.Cookie.IsEssential = redisSessionStorageOption.Cookie.HttpOnly;
            options.Cookie.SecurePolicy = (CookieSecurePolicy)
                Enum.ToObject(
                    enumType: typeof(CookieSecurePolicy),
                    value: redisSessionStorageOption.Cookie.SecurePolicy
                );
        });
    }
}
