using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.Order.Entities;
using static ODour.PostgresRelationalDb.Common.CommonConstant;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class OrderItemEntityConfiguration : IEntityTypeConfiguration<OrderItemEntity>
{
    public void Configure(EntityTypeBuilder<OrderItemEntity> builder)
    {
        builder.ToTable(
            name: OrderItemEntity.MetaData.TableName,
            schema: $"{DatabaseSchemaName.MAIN}.{DatabaseSchemaName.ORDER}",
            buildAction: table => table.HasComment(comment: "Contain order items.")
        );

        builder.HasKey(keyExpression: builder => new { builder.OrderId, builder.ProductId });

        builder
            .Property(propertyExpression: builder => builder.ProductId)
            .HasMaxLength(maxLength: OrderItemEntity.MetaData.ProductId.MaxLength);

        builder
            .Property(propertyExpression: builder => builder.SellingPrice)
            .HasPrecision(
                precision: OrderItemEntity.MetaData.SellingPrice.Precision,
                scale: OrderItemEntity.MetaData.SellingPrice.Scale
            )
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.SellingQuantity)
            .IsRequired(required: true);

        #region Relationships
        builder
            .HasOne(navigationExpression: orderItem => orderItem.Order)
            .WithMany(navigationExpression: order => order.OrderItems)
            .HasForeignKey(foreignKeyExpression: orderItem => orderItem.OrderId)
            .OnDelete(deleteBehavior: DeleteBehavior.Cascade);

        builder
            .HasOne(navigationExpression: order => order.Product)
            .WithMany(navigationExpression: product => product.OrderItems)
            .HasForeignKey(foreignKeyExpression: order => order.ProductId)
            .OnDelete(deleteBehavior: DeleteBehavior.Cascade);
        #endregion
    }
}
