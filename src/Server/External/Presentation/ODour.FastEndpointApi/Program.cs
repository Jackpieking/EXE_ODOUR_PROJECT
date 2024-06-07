using System;
using System.Text;
using System.Threading;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.JsonWebTokens;
using ODour.Application.Share.DataProtection;
using ODour.Domain.Share.Role.Entities;
using ODour.Domain.Share.User.Entities;
using ODour.FastEndpointApi.ServiceConfigs;
using ODour.FastEndpointApi.Shared.Middlewares;
using ODour.PostgresRelationalDb.Data;

// Comment this line if switch to another database.
AppContext.SetSwitch(switchName: "Npgsql.DisableDateTimeInfinityConversions", isEnabled: true);

Console.OutputEncoding = Encoding.UTF8;
JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args: args);

// Add services to the container.
var services = builder.Services;
var configuration = builder.Configuration;

PostgresRelationalDbServiceConfig.Config(services: services, configuration: configuration);
WebApiServiceConfig.Config(services: services, configuration: configuration);
RedisCachingDbServiceConfig.Config(services: services, configuration: configuration);
CustomServiceConfig.Config(services: services, configuration: configuration);
AppNotificationServiceConfig.Config(services: services, configuration: configuration);
AppIdentityServiceConfig.Config(services: services, configuration: configuration);
ApplicationServiceConfig.Config(services: services, configuration: configuration);
AppBackgroundJobServiceConfig.Config(services: services, configuration: configuration);

var app = builder.Build();

// Data seeding.
await using (var scope = app.Services.CreateAsyncScope())
{
    var context = scope.TryResolve<DbContext>();

    // Can database be connected.
    var canConnect = await context.Database.CanConnectAsync();

    // Database cannot be connected.
    if (!canConnect)
    {
        throw new HostAbortedException(message: "Cannot connect database.");
    }

    // Try seed data.
    var seedResult = await EntityDataSeeding.SeedAsync(
        context: context,
        userManager: scope.TryResolve<Lazy<UserManager<UserEntity>>>(),
        roleManager: scope.TryResolve<Lazy<RoleManager<RoleEntity>>>(),
        dataProtectionHandler: scope.TryResolve<Lazy<IDataProtectionHandler>>(),
        cancellationToken: CancellationToken.None
    );

    // Data cannot be seed.
    if (!seedResult)
    {
        throw new HostAbortedException(message: "Database seeding is false.");
    }
}

// Configure the HTTP request pipeline.
app.UseAppExceptionHandler()
    .UseCors()
    .UseAuthentication()
    .UseAuthorization()
    .UseResponseCaching()
    .UseFastEndpoints()
    .UseSwaggerGen()
    .UseSwaggerUi(configure: options =>
    {
        options.Path = string.Empty;
        options.DefaultModelsExpandDepth = -1;
    })
    .UseJobQueues(options: options =>
    {
        options.MaxConcurrency = 5;
        options.StorageProbeDelay = TimeSpan.FromSeconds(value: 5);
        options.ExecutionTimeLimit = TimeSpan.FromMinutes(value: 2);
    });

// .UseHangfireDashboard(
//     options: new()
//     {
//         DashboardTitle = "ODour Hangfire Dashboard",
//         DisplayStorageConnectionString = false,
//         Authorization = new[]
//         {
//             app.Services.GetRequiredService<IDashboardAuthorizationFilter>()
//         }
//     }
// );

// Clear all current allocations.
GC.Collect();

await app.RunAsync();
