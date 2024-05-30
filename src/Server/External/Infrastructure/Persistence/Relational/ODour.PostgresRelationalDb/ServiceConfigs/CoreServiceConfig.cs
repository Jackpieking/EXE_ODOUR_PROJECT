using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ODour.Application.Share.Common;
using ODour.Domain.Feature.Main;
using ODour.Domain.Share.Role.Entities;
using ODour.Domain.Share.User.Entities;
using ODour.PostgresRelationalDb.Main;

namespace ODour.PostgresRelationalDb.ServiceConfigs;

internal static class CoreServiceConfig
{
    internal static void Config(IServiceCollection services)
    {
        // ====
        services.MakeScopedLazy<DbContext>();

        // ====
        services.AddScoped<IMainUnitOfWork, MainUnitOfWork>();
        services.MakeScopedLazy<IMainUnitOfWork>();

        // ====
        services.MakeScopedLazy<UserManager<UserEntity>>();

        // ====
        services.MakeScopedLazy<RoleManager<RoleEntity>>();
    }
}
