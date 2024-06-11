using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.Product.Entities;
using static ODour.PostgresRelationalDb.Common.CommonConstant;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class ProductEntityConfiguration : IEntityTypeConfiguration<ProductEntity>
{
    public void Configure(EntityTypeBuilder<ProductEntity> builder)
    {
        builder.ToTable(
            name: ProductEntity.MetaData.TableName,
            schema: $"{DatabaseSchemaName.MAIN}.{DatabaseSchemaName.PRODUCT}",
            buildAction: table => table.HasComment(comment: "Contain products.")
        );

        builder.HasKey(keyExpression: builder => builder.Id);

        builder
            .Property(propertyExpression: builder => builder.Id)
            .HasMaxLength(maxLength: ProductEntity.MetaData.Id.MaxLength);

        builder
            .Property(propertyExpression: builder => builder.ProductStatusId)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.CategoryId)
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
            .HasColumnType(typeName: DatabaseNativeType.JSONB)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.QuantityInStock)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.IsTemporarilyRemoved)
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
        #endregion
    }
}
