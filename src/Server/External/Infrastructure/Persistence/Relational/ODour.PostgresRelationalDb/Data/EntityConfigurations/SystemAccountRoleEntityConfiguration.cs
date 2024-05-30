using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.System.Entities;
using static ODour.PostgresRelationalDb.Common.CommonConstant;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations
{
    internal sealed class SystemAccountRoleEntityConfiguration
        : IEntityTypeConfiguration<SystemAccountRoleEntity>
    {
        public void Configure(EntityTypeBuilder<SystemAccountRoleEntity> builder)
        {
            builder.ToTable(
                name: SystemAccountRoleEntity.MetaData.TableName,
                schema: $"{DatabaseSchemaName.MAIN}.{DatabaseSchemaName.SYSTEM}",
                buildAction: table => table.HasComment(comment: "Contain system account roles.")
            );

            builder.HasKey(keyExpression: builder => new
            {
                builder.SystemAccountId,
                builder.RoleId
            });

            #region Relationships
            builder
                .HasOne(navigationExpression: systemAccountRole => systemAccountRole.SystemAccount)
                .WithMany(navigationExpression: systemAccount => systemAccount.SystemAccountRoles)
                .HasForeignKey(foreignKeyExpression: systemAccountRole =>
                    systemAccountRole.SystemAccountId
                )
                .OnDelete(deleteBehavior: DeleteBehavior.NoAction);

            builder
                .HasOne(navigationExpression: systemAccountRole => systemAccountRole.Role)
                .WithMany(navigationExpression: role => role.SystemAccountRoles)
                .HasForeignKey(foreignKeyExpression: systemAccountRole => systemAccountRole.RoleId)
                .OnDelete(deleteBehavior: DeleteBehavior.NoAction);
            #endregion
        }
    }
}
