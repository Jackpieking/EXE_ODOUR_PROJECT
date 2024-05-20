using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.Entities;
using ODour.PostgresRelationalDb.Shared;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class RoleDetailEntityConfiguration : IEntityTypeConfiguration<RoleDetailEntity>
{
    public void Configure(EntityTypeBuilder<RoleDetailEntity> builder)
    {
        builder.ToTable(
            name: RoleDetailEntity.MetaData.TableName,
            buildAction: table => table.HasComment(comment: "Contain role details.")
        );

        builder.HasKey(keyExpression: builder => builder.RoleId);

        builder
            .Property(propertyExpression: builder => builder.CreatedAt)
            .HasColumnType(typeName: CommonConstant.DatabaseNativeType.TIMESTAMPTZ)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.CreatedBy)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.UpdatedAt)
            .HasColumnType(typeName: CommonConstant.DatabaseNativeType.TIMESTAMPTZ)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.UpdatedBy)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.RemovedAt)
            .HasColumnType(typeName: CommonConstant.DatabaseNativeType.TIMESTAMPTZ)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.RemovedBy)
            .IsRequired(required: true);

        #region Relationships
        builder
            .HasOne(navigationExpression: roleDetail => roleDetail.Creator)
            .WithMany(navigationExpression: systemAccount => systemAccount.RoleDetailCreators)
            .HasForeignKey(foreignKeyExpression: roleDetail => roleDetail.CreatedBy)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);

        builder
            .HasOne(navigationExpression: roleDetail => roleDetail.Updater)
            .WithMany(navigationExpression: systemAccount => systemAccount.RoleDetailUpdaters)
            .HasForeignKey(foreignKeyExpression: roleDetail => roleDetail.UpdatedBy)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);

        builder
            .HasOne(navigationExpression: roleDetail => roleDetail.Remover)
            .WithMany(navigationExpression: systemAccount => systemAccount.RoleDetailRemovers)
            .HasForeignKey(foreignKeyExpression: roleDetail => roleDetail.RemovedBy)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);

        builder
            .HasOne(navigationExpression: roleDetail => roleDetail.Role)
            .WithOne(navigationExpression: role => role.RoleDetail)
            .HasForeignKey<RoleDetailEntity>(foreignKeyExpression: roleDetail => roleDetail.RoleId)
            .OnDelete(deleteBehavior: DeleteBehavior.Cascade);
        #endregion
    }
}
