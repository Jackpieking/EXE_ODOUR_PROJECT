using Microsoft.AspNetCore.Identity;
using ODour.Domain.Share.Entities;

namespace ODour.PostgresRelationalDb.Data;

public static class EntityDataSeeding
{
    public static bool SeedAsync(
        ODourContext context,
        UserManager<UserEntity> userManager,
        RoleManager<RoleEntity> roleManager
    )
    {
        return true;
    }
}
