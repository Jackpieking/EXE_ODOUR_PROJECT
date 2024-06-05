using System;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ODour.Configuration.Infrastructure.BackgroundJob;
using ODour.Configuration.Infrastructure.Persistence.Database;

namespace ODour.AppBackgroundJob.ServiceConfigs;

internal static class AppBackgroundJobServiceConfig
{
    internal static void Config(IServiceCollection services, IConfiguration configuration)
    {
        var databaseOption = configuration
            .GetRequiredSection(key: "Database")
            .GetRequiredSection(key: "ODourMainDb")
            .Get<ODourDatabaseOption>();

        var hangfireOption = configuration
            .GetRequiredSection(key: "BackgroundJob")
            .GetRequiredSection(key: "HangFire")
            .Get<HangFireOption>();

        services
            .AddHangfire(configuration: configuration =>
            {
                configuration
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UsePostgreSqlStorage(configure: action =>
                    {
                        action.UseNpgsqlConnection(
                            connectionString: databaseOption.ConnectionString
                        );
                    });
            })
            .AddHangfireServer(optionsAction: option =>
            {
                option.SchedulePollingInterval = TimeSpan.FromSeconds(
                    value: hangfireOption.SchedulePollingIntervalInSeconds
                );
                option.ServerName = hangfireOption.ServerName;
            });
    }
}
