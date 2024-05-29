using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.Voucher.Entities;
using static ODour.PostgresRelationalDb.Common.CommonConstant;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class VoucherEntityConfiguration : IEntityTypeConfiguration<VoucherEntity>
{
    public void Configure(EntityTypeBuilder<VoucherEntity> builder)
    {
        builder.ToTable(
            name: VoucherEntity.MetaData.TableName,
            schema: $"{DatabaseSchemaName.MAIN}.{DatabaseSchemaName.VOUCHER}",
            buildAction: table => table.HasComment(comment: "Contain vouchers.")
        );

        builder.HasKey(keyExpression: builder => builder.Code);

        builder
            .Property(propertyExpression: builder => builder.Code)
            .HasMaxLength(maxLength: VoucherEntity.MetaData.Code.MaxLength)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.Name)
            .HasMaxLength(maxLength: VoucherEntity.MetaData.Name.MaxLength)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.QuantityInStock)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.DiscountPercentage)
            .HasPrecision(
                precision: VoucherEntity.MetaData.DiscountPercentage.Precision,
                scale: VoucherEntity.MetaData.DiscountPercentage.Scale
            )
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.Description)
            .HasColumnType(typeName: DatabaseNativeType.TEXT)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.DueDate)
            .HasColumnType(typeName: DatabaseNativeType.TIMESTAMPTZ)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.StartDate)
            .HasColumnType(typeName: DatabaseNativeType.TIMESTAMPTZ)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.IsTemporarilyRemoved)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.VoucherTypeId)
            .IsRequired(required: true);

        #region Relationships
        builder
            .HasOne(navigationExpression: voucher => voucher.VoucherType)
            .WithMany(navigationExpression: productVoucherType => productVoucherType.Vouchers)
            .HasForeignKey(foreignKeyExpression: voucher => voucher.VoucherTypeId)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);
        #endregion
    }
}
