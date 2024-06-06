using System;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ODour.Configuration.Infrastructure.BackgroundJob;
using ODour.Configuration.Infrastructure.Persistence.Database;

namespace ODour.FastEndpointApi.ServiceConfigs;

internal static class AppBackgroundJobServiceConfig
{
    internal static IServiceCollection Config(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // ====
        var hangfireOption = configuration
            .GetRequiredSection(key: "BackgroundJob")
            .GetRequiredSection(key: "HangFire")
            .Get<HangFireOption>();

        var dbOption = configuration
            .GetRequiredSection(key: "Database")
            .GetRequiredSection(key: "ODourMainDb")
            .Get<ODourDatabaseOption>();

        services
            .AddHangfire(configuration: configuration =>
            {
                configuration
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UsePostgreSqlStorage(configure: action =>
                        action.UseNpgsqlConnection(connectionString: dbOption.ConnectionString)
                    );
            })
            .AddHangfireServer(optionsAction: option =>
            {
                option.SchedulePollingInterval = TimeSpan.FromSeconds(
                    value: hangfireOption.SchedulePollingIntervalInSeconds
                );
                option.ServerName = hangfireOption.ServerName;
                option.WorkerCount = 5;
            });

        return services;
    }
}
