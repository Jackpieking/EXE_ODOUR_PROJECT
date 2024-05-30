using System;
using System.Text;
using System.Threading;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.JsonWebTokens;
using ODour.AppBackgroundJob;
using ODour.AppIdentityService;
using ODour.Application;
using ODour.AppNotification;
using ODour.Configuration.Presentation.WebApi.SecurityKey;
using ODour.Domain.Share.Role.Entities;
using ODour.Domain.Share.User.Entities;
using ODour.FastEndpointApi;
using ODour.FastEndpointApi.Shared.Middlewares;
using ODour.PostgresRelationalDb;
using ODour.PostgresRelationalDb.Data;

// Comment this line if switch to another database.
AppContext.SetSwitch(switchName: "Npgsql.DisableDateTimeInfinityConversions", isEnabled: true);

Console.OutputEncoding = Encoding.UTF8;
JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args: args);

// Add services to the container.
var services = builder.Services;
var configuration = builder.Configuration;

services.AddOdourPostgresRelationalDb(configuration: configuration);
services.AddApplication();
services.AddWebApi(configuration: configuration);
services.AddAppIdentityService();
services.AddAppNotification();
services.AddAppBackgroundJob();

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
        dataProtectionProvider: scope.TryResolve<Lazy<IDataProtectionProvider>>(),
        protectionSecurityKeyOption: scope.TryResolve<Lazy<AppBaseProtectionSecurityKeyOption>>(),
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
    });

// Clear all current allocations.
GC.Collect();

await app.RunAsync();
