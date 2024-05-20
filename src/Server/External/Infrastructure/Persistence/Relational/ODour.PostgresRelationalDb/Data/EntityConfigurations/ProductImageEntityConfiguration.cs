using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.Entities;
using ODour.PostgresRelationalDb.Shared;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class ProductImageEntityConfiguration : IEntityTypeConfiguration<ProductImageEntity>
{
    public void Configure(EntityTypeBuilder<ProductImageEntity> builder)
    {
        builder.ToTable(
            name: ProductImageEntity.MetaData.TableName,
            buildAction: table => table.HasComment(comment: "Contain product images.")
        );

        builder.HasKey(keyExpression: builder => builder.Id);

        builder
            .Property(propertyExpression: builder => builder.ProductId)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.UploadOrder)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.StorageUrl)
            .HasColumnType(typeName: CommonConstant.DatabaseNativeType.TEXT)
            .IsRequired(required: true);

        #region Relationships
        builder
            .HasOne(navigationExpression: productImage => productImage.Product)
            .WithMany(navigationExpression: product => product.ProductImages)
            .HasForeignKey(foreignKeyExpression: productImage => productImage.ProductId)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);
        #endregion
    }
}
