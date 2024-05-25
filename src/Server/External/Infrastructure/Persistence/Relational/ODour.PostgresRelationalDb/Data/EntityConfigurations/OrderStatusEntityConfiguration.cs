using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.Order.Entities;
using ODour.PostgresRelationalDb.Common;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class OrderStatusEntityConfiguration : IEntityTypeConfiguration<OrderStatusEntity>
{
    public void Configure(EntityTypeBuilder<OrderStatusEntity> builder)
    {
        builder.ToTable(
            name: OrderStatusEntity.MetaData.TableName,
            schema: $"{CommonConstant.DatabaseSchemaName.MAIN}.{CommonConstant.DatabaseSchemaName.ORDER}",
            buildAction: table => table.HasComment(comment: "Contain order statuses.")
        );

        builder.HasKey(keyExpression: builder => builder.Id);

        builder
            .Property(propertyExpression: builder => builder.Name)
            .HasMaxLength(maxLength: OrderStatusEntity.MetaData.Name.MaxLength)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.IsTemporarilyRemoved)
            .IsRequired(required: true);
    }
}
