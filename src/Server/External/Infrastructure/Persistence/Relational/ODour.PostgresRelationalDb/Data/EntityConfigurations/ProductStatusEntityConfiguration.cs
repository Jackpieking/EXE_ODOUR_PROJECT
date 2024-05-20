using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.Entities;
using ODour.PostgresRelationalDb.Shared;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class ProductStatusEntityConfiguration
    : IEntityTypeConfiguration<ProductStatusEntity>
{
    public void Configure(EntityTypeBuilder<ProductStatusEntity> builder)
    {
        builder.ToTable(
            name: ProductStatusEntity.MetaData.TableName,
            buildAction: table => table.HasComment(comment: "Contain product statuses.")
        );

        builder.HasKey(keyExpression: builder => builder.Id);

        builder
            .Property(propertyExpression: builder => builder.Name)
            .HasMaxLength(maxLength: ProductStatusEntity.MetaData.Name.MaxLength)
            .IsRequired(required: true);

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
            .HasOne(navigationExpression: productStatus => productStatus.Creator)
            .WithMany(navigationExpression: systemAccount => systemAccount.ProductStatusCreators)
            .HasForeignKey(foreignKeyExpression: productStatus => productStatus.CreatedBy)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);

        builder
            .HasOne(navigationExpression: productStatus => productStatus.Updater)
            .WithMany(navigationExpression: systemAccount => systemAccount.ProductStatusUpdaters)
            .HasForeignKey(foreignKeyExpression: productStatus => productStatus.UpdatedBy)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);

        builder
            .HasOne(navigationExpression: productStatus => productStatus.Remover)
            .WithMany(navigationExpression: systemAccount => systemAccount.ProductStatusRemovers)
            .HasForeignKey(foreignKeyExpression: productStatus => productStatus.RemovedBy)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);
        #endregion
    }
}
