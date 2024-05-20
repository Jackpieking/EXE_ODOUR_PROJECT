using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.Entities;
using ODour.PostgresRelationalDb.Shared;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class OrderStatusEntityConfiguration : IEntityTypeConfiguration<OrderStatusEntity>
{
    public void Configure(EntityTypeBuilder<OrderStatusEntity> builder)
    {
        builder.ToTable(
            name: OrderStatusEntity.MetaData.TableName,
            buildAction: table => table.HasComment(comment: "Contain order statuses.")
        );

        builder.HasKey(keyExpression: builder => builder.Id);

        builder
            .Property(propertyExpression: builder => builder.Name)
            .HasMaxLength(maxLength: OrderStatusEntity.MetaData.Name.MaxLength)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.CreatedAt)
            .HasColumnType(typeName: CommonConstant.DatabaseNativeType.TIMESTAMPTZ)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.CreatedBy)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.UpdatedAt)
            .HasColumnType(typeName: CommonConstant.DatabaseNativeType.TIMESTAMPTZ)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.UpdatedBy)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.RemovedAt)
            .HasColumnType(typeName: CommonConstant.DatabaseNativeType.TIMESTAMPTZ)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.RemovedBy)
            .IsRequired(required: true);

        #region Relationships
        builder
            .HasOne(navigationExpression: orderStatus => orderStatus.Creator)
            .WithMany(navigationExpression: systemAccount => systemAccount.OrderStatusCreators)
            .HasForeignKey(foreignKeyExpression: orderStatus => orderStatus.CreatedBy)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);

        builder
            .HasOne(navigationExpression: orderStatus => orderStatus.Updater)
            .WithMany(navigationExpression: systemAccount => systemAccount.OrderStatusUpdaters)
            .HasForeignKey(foreignKeyExpression: orderStatus => orderStatus.UpdatedBy)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);

        builder
            .HasOne(navigationExpression: orderStatus => orderStatus.Remover)
            .WithMany(navigationExpression: systemAccount => systemAccount.OrderStatusRemovers)
            .HasForeignKey(foreignKeyExpression: orderStatus => orderStatus.RemovedBy)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);
        #endregion
    }
}
