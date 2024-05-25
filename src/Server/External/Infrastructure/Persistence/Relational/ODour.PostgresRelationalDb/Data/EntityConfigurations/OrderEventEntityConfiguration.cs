using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.Order.Entities;
using ODour.PostgresRelationalDb.Common;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class OrderEventEntityConfiguration : IEntityTypeConfiguration<OrderEventEntity>
{
    public void Configure(EntityTypeBuilder<OrderEventEntity> builder)
    {
        builder.ToTable(
            name: OrderEventEntity.MetaData.TableName,
            schema: $"{CommonConstant.DatabaseSchemaName.MAIN}.{CommonConstant.DatabaseSchemaName.ORDER}",
            buildAction: table => table.HasComment(comment: "Contain order events.")
        );

        builder.HasKey(keyExpression: builder => builder.Id);

        builder
            .Property(propertyExpression: builder => builder.Type)
            .HasMaxLength(maxLength: OrderEventEntity.MetaData.Type.MaxLength)
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
            .HasOne(navigationExpression: orderEvent => orderEvent.Creator)
            .WithMany(navigationExpression: user => user.OrderEventCreators)
            .HasForeignKey(foreignKeyExpression: orderEvent => orderEvent.CreatedBy)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);
        #endregion
    }
}
