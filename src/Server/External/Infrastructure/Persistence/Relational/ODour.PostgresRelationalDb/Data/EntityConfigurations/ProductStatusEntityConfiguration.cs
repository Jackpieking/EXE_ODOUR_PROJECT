using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.Product.Entities;
using ODour.PostgresRelationalDb.Common;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class ProductStatusEntityConfiguration
    : IEntityTypeConfiguration<ProductStatusEntity>
{
    public void Configure(EntityTypeBuilder<ProductStatusEntity> builder)
    {
        builder.ToTable(
            name: ProductStatusEntity.MetaData.TableName,
            schema: $"{CommonConstant.DatabaseSchemaName.MAIN}.{CommonConstant.DatabaseSchemaName.PRODUCT}",
            buildAction: table => table.HasComment(comment: "Contain product statuses.")
        );

        builder.HasKey(keyExpression: builder => builder.Id);

        builder
            .Property(propertyExpression: builder => builder.Name)
            .HasMaxLength(maxLength: ProductStatusEntity.MetaData.Name.MaxLength)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.IsTemporarilyRemoved)
            .IsRequired(required: true);
    }
}
