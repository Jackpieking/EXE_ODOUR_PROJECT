using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.Product.Entities;
using static ODour.PostgresRelationalDb.Common.CommonConstant;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class ProductMediaEntityConfiguration : IEntityTypeConfiguration<ProductMediaEntity>
{
    public void Configure(EntityTypeBuilder<ProductMediaEntity> builder)
    {
        builder.ToTable(
            name: ProductMediaEntity.MetaData.TableName,
            schema: $"{DatabaseSchemaName.MAIN}.{DatabaseSchemaName.PRODUCT}",
            buildAction: table => table.HasComment(comment: "Contain product medias.")
        );

        builder.HasKey(keyExpression: builder => builder.Id);

        builder
            .Property(propertyExpression: builder => builder.ProductId)
            .HasMaxLength(maxLength: ProductMediaEntity.MetaData.ProductId.MaxLength)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.UploadOrder)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.StorageUrl)
            .HasColumnType(typeName: DatabaseNativeType.TEXT)
            .IsRequired(required: true);

        #region Relationships
        builder
            .HasOne(navigationExpression: productImage => productImage.Product)
            .WithMany(navigationExpression: product => product.ProductMedias)
            .HasForeignKey(foreignKeyExpression: productMedia => productMedia.ProductId)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);
        #endregion
    }
}
