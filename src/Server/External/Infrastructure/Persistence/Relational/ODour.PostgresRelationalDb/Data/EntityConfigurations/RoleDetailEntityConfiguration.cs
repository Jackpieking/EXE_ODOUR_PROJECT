using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.Role.Entities;
using ODour.PostgresRelationalDb.Common;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class RoleDetailEntityConfiguration : IEntityTypeConfiguration<RoleDetailEntity>
{
    public void Configure(EntityTypeBuilder<RoleDetailEntity> builder)
    {
        builder.ToTable(
            name: RoleDetailEntity.MetaData.TableName,
            schema: $"{CommonConstant.DatabaseSchemaName.MAIN}.{CommonConstant.DatabaseSchemaName.ROLE}",
            buildAction: table => table.HasComment(comment: "Contain role details.")
        );

        builder.HasKey(keyExpression: builder => builder.RoleId);

        builder
            .Property(propertyExpression: builder => builder.IsTemporarilyRemoved)
            .IsRequired(required: true);

        #region Relationships
        builder
            .HasOne(navigationExpression: roleDetail => roleDetail.Role)
            .WithOne(navigationExpression: role => role.RoleDetail)
            .HasForeignKey<RoleDetailEntity>(foreignKeyExpression: roleDetail => roleDetail.RoleId)
            .OnDelete(deleteBehavior: DeleteBehavior.Cascade);
        #endregion
    }
}
