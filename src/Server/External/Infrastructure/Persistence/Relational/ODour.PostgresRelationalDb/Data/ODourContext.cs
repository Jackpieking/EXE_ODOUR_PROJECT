using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ODour.Domain.Share.Entities;

namespace ODour.PostgresRelationalDb.Data;

public sealed class ODourContext : IdentityDbContext<UserEntity, RoleEntity, Guid>
{
    public ODourContext(DbContextOptions<ODourContext> options)
        : base(options: options) { }

    /// <summary>
    ///     Configure tables and seed initial data here.
    /// </summary>
    /// <param name="builder">
    ///     Model builder access the database.
    /// </param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder: builder);

        RemoveAspNetPrefixInIdentityTable(builder: builder);

        InitCaseInsensitiveCollation(builder: builder);

        builder.ApplyConfigurationsFromAssembly(assembly: typeof(ODourContext).Assembly);
    }

    /// <summary>
    ///     Remove "AspNet" prefix in identity table name.
    /// </summary>
    /// <param name="builder">
    ///     Model builder access the database.
    /// </param>
    private static void RemoveAspNetPrefixInIdentityTable(ModelBuilder builder)
    {
        const string AspNetPrefix = "AspNet";

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();

            if (tableName.StartsWith(value: AspNetPrefix))
            {
                entityType.SetTableName(name: tableName[6..]);
            }
        }
    }

    /// <summary>
    ///     Create new case insensitive collation.
    /// </summary>
    /// <param name="builder">
    ///     Model builder access the database.
    /// </param>
    private static void InitCaseInsensitiveCollation(ModelBuilder builder)
    {
        builder.HasCollation(
            name: "case_insensitive",
            locale: "en-u-ks-primary",
            provider: "icu",
            deterministic: false
        );
    }
}
