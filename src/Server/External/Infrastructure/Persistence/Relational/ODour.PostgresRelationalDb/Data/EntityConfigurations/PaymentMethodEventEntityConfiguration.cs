using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.Payment.Entities;
using ODour.PostgresRelationalDb.Common;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class PaymentMethodEventEntityConfiguration
    : IEntityTypeConfiguration<PaymentMethodEventEntity>
{
    public void Configure(EntityTypeBuilder<PaymentMethodEventEntity> builder)
    {
        builder.ToTable(
            name: PaymentMethodEventEntity.MetaData.TableName,
            schema: $"{CommonConstant.DatabaseSchemaName.MAIN}.{CommonConstant.DatabaseSchemaName.PAYMENT}",
            buildAction: table => table.HasComment(comment: "Contain payment method events.")
        );

        builder.HasKey(keyExpression: builder => builder.Id);

        builder
            .Property(propertyExpression: builder => builder.Type)
            .HasMaxLength(maxLength: PaymentMethodEventEntity.MetaData.Type.MaxLength)
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
            .HasOne(navigationExpression: paymentMethodEvent => paymentMethodEvent.Creator)
            .WithMany(navigationExpression: systemAccount =>
                systemAccount.PaymentMethodEventCreators
            )
            .HasForeignKey(foreignKeyExpression: paymentMethodEvent => paymentMethodEvent.CreatedBy)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);
        #endregion
    }
}
