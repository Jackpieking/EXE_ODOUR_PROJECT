using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.Product.Entities;
using ODour.PostgresRelationalDb.Common;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class ProductStatusEventEntityConfigurationv
    : IEntityTypeConfiguration<ProductStatusEventEntity>
{
    public void Configure(EntityTypeBuilder<ProductStatusEventEntity> builder)
    {
        builder.ToTable(
            name: ProductStatusEventEntity.MetaData.TableName,
            schema: $"{CommonConstant.DatabaseSchemaName.MAIN}.{CommonConstant.DatabaseSchemaName.PRODUCT}",
            buildAction: table => table.HasComment(comment: "Contain product status events.")
        );

        builder.HasKey(keyExpression: builder => builder.Id);

        builder
            .Property(propertyExpression: builder => builder.Type)
            .HasMaxLength(maxLength: ProductStatusEventEntity.MetaData.Type.MaxLength)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.StreamId)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.OldData)
            .HasColumnType(typeName: CommonConstant.DatabaseNativeType.TEXT)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.NewData)
            .HasColumnType(typeName: CommonConstant.DatabaseNativeType.TEXT)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.CreatedAt)
            .HasColumnType(typeName: CommonConstant.DatabaseNativeType.TIMESTAMPTZ)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.CreatedBy)
            .IsRequired(required: true);

        #region Relationships
        builder
            .HasOne(navigationExpression: productStatusEvent => productStatusEvent.Creator)
            .WithMany(navigationExpression: systemAccount =>
                systemAccount.ProductStatusEventCreators
            )
            .HasForeignKey(foreignKeyExpression: productStatusEvent => productStatusEvent.CreatedBy)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);
        #endregion
    }
}
