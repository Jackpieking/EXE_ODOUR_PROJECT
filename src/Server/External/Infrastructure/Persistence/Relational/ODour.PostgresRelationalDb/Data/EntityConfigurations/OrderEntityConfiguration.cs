using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.Order.Entities;
using static ODour.PostgresRelationalDb.Common.CommonConstant;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class OrderEntityConfiguration : IEntityTypeConfiguration<OrderEntity>
{
    public void Configure(EntityTypeBuilder<OrderEntity> builder)
    {
        builder.ToTable(
            name: OrderEntity.MetaData.TableName,
            schema: $"{DatabaseSchemaName.MAIN}.{DatabaseSchemaName.ORDER}",
            buildAction: table => table.HasComment(comment: "Contain orders.")
        );

        builder.HasKey(keyExpression: builder => builder.Id);

        builder
            .Property(propertyExpression: builder => builder.OrderStatusId)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.PaymentMethodId)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.OrderCode)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.OrderNote)
            .HasMaxLength(maxLength: OrderEntity.MetaData.OrderNote.MaxLength)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.TotalPrice)
            .HasPrecision(
                precision: OrderEntity.MetaData.TotalPrice.Precision,
                scale: OrderEntity.MetaData.TotalPrice.Scale
            )
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.DeliveredAddress)
            .HasMaxLength(maxLength: OrderEntity.MetaData.DeliveredAddress.MaxLength)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.DeliveredAt)
            .HasColumnType(typeName: DatabaseNativeType.TIMESTAMPTZ)
            .IsRequired(required: true);

        #region Relationships
        builder
            .HasOne(navigationExpression: order => order.PaymentMethod)
            .WithMany(navigationExpression: paymentMethod => paymentMethod.Orders)
            .HasForeignKey(foreignKeyExpression: order => order.PaymentMethodId)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);

        builder
            .HasOne(navigationExpression: order => order.OrderStatus)
            .WithMany(navigationExpression: orderStatus => orderStatus.Orders)
            .HasForeignKey(foreignKeyExpression: order => order.OrderStatusId)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);
        #endregion
    }
}
