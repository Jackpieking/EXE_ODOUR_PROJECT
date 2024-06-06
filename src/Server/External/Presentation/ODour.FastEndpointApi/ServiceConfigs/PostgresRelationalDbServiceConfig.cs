using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ODour.Configuration.Infrastructure.Persistence.AspNetCoreIdentity;
using ODour.Configuration.Infrastructure.Persistence.Database;
using ODour.Domain.Share.Role.Entities;
using ODour.Domain.Share.User.Entities;
using ODour.PostgresRelationalDb.Data;

namespace ODour.FastEndpointApi.ServiceConfigs;

internal static class PostgresRelationalDbServiceConfig
{
    internal static IServiceCollection Config(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContextPool<DbContext, ODourContext>(optionsAction: config =>
        {
            var option = configuration
                .GetRequiredSection(key: "Database")
                .GetRequiredSection(key: "ODourMainDb")
                .Get<ODourDatabaseOption>();

            config
                .UseNpgsql(
                    connectionString: option.ConnectionString,
                    npgsqlOptionsAction: npgsqlOptionsAction =>
                    {
                        npgsqlOptionsAction
                            .CommandTimeout(commandTimeout: option.CommandTimeOut)
                            .EnableRetryOnFailure(maxRetryCount: option.EnableRetryOnFailure)
                            .MigrationsAssembly(
                                assemblyName: typeof(ODourContext).Assembly.FullName
                            );
                    }
                )
                .EnableSensitiveDataLogging(
                    sensitiveDataLoggingEnabled: option.EnableSensitiveDataLogging
                )
                .EnableDetailedErrors(detailedErrorsEnabled: option.EnableDetailedErrors)
                .EnableThreadSafetyChecks(enableChecks: option.EnableThreadSafetyChecks)
                .EnableServiceProviderCaching(
                    cacheServiceProvider: option.EnableServiceProviderCaching
                );
        });

        // ====
        services
            .AddIdentity<UserEntity, RoleEntity>(setupAction: config =>
            {
                var option = configuration
                    .GetRequiredSection(key: "AspNetCoreIdentity")
                    .Get<AspNetCoreIdentityOption>();

                config.Password.RequireDigit = option.Password.RequireDigit;
                config.Password.RequireLowercase = option.Password.RequireLowercase;
                config.Password.RequireNonAlphanumeric = option.Password.RequireNonAlphanumeric;
                config.Password.RequireUppercase = option.Password.RequireUppercase;
                config.Password.RequiredLength = option.Password.RequiredLength;
                config.Password.RequiredUniqueChars = option.Password.RequiredUniqueChars;

                config.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(
                    value: option.Lockout.DefaultLockoutTimeSpanInSecond
                );
                config.Lockout.MaxFailedAccessAttempts = option.Lockout.MaxFailedAccessAttempts;
                config.Lockout.AllowedForNewUsers = option.Lockout.AllowedForNewUsers;

                config.User.AllowedUserNameCharacters = option.User.AllowedUserNameCharacters;
                config.User.RequireUniqueEmail = option.User.RequireUniqueEmail;

                config.SignIn.RequireConfirmedEmail = option.SignIn.RequireConfirmedEmail;
                config.SignIn.RequireConfirmedPhoneNumber = option
                    .SignIn
                    .RequireConfirmedPhoneNumber;
            })
            .AddEntityFrameworkStores<ODourContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}
