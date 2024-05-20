using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.Entities;
using ODour.PostgresRelationalDb.Shared;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class ProductEntityConfiguration : IEntityTypeConfiguration<ProductEntity>
{
    public void Configure(EntityTypeBuilder<ProductEntity> builder)
    {
        builder.ToTable(
            name: ProductEntity.MetaData.TableName,
            buildAction: table => table.HasComment(comment: "Contain products.")
        );

        builder.HasKey(keyExpression: builder => builder.Id);

        builder
            .Property(propertyExpression: builder => builder.ProductStatusId)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.CategoryId)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.CreatedBy)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.UpdatedBy)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.RemovedBy)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.Name)
            .HasMaxLength(maxLength: ProductEntity.MetaData.Name.MaxLength)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.UnitPrice)
            .HasPrecision(
                precision: ProductEntity.MetaData.UnitPrice.Precision,
                scale: ProductEntity.MetaData.UnitPrice.Scale
            )
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.Description)
            .HasColumnType(typeName: CommonConstant.DatabaseNativeType.TEXT)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.QuantityInStock)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.CreatedAt)
            .HasColumnType(typeName: CommonConstant.DatabaseNativeType.TIMESTAMPTZ)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.UpdatedAt)
            .HasColumnType(typeName: CommonConstant.DatabaseNativeType.TIMESTAMPTZ)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.RemovedAt)
            .HasColumnType(typeName: CommonConstant.DatabaseNativeType.TIMESTAMPTZ)
            .IsRequired(required: true);

        #region Relationships
        builder
            .HasOne(navigationExpression: product => product.ProductStatus)
            .WithMany(navigationExpression: productStatus => productStatus.Products)
            .HasForeignKey(foreignKeyExpression: product => product.ProductStatusId)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);

        builder
            .HasOne(navigationExpression: product => product.Category)
            .WithMany(navigationExpression: category => category.Products)
            .HasForeignKey(foreignKeyExpression: product => product.CategoryId)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);

        builder
            .HasOne(navigationExpression: product => product.Creator)
            .WithMany(navigationExpression: systemAccount => systemAccount.ProductCreators)
            .HasForeignKey(foreignKeyExpression: product => product.CreatedBy)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);

        builder
            .HasOne(navigationExpression: product => product.Updater)
            .WithMany(navigationExpression: systemAccount => systemAccount.ProductUpdaters)
            .HasForeignKey(foreignKeyExpression: product => product.UpdatedBy)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);

        builder
            .HasOne(navigationExpression: accountStatus => accountStatus.Remover)
            .WithMany(navigationExpression: systemAccount => systemAccount.ProductRemovers)
            .HasForeignKey(foreignKeyExpression: accountStatus => accountStatus.RemovedBy)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);
        #endregion
    }
}
