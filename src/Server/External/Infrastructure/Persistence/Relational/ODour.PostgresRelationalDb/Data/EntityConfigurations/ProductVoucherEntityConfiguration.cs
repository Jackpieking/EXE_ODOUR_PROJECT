using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.Voucher.Entities;
using static ODour.PostgresRelationalDb.Common.CommonConstant;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class ProductVoucherEntityConfiguration
    : IEntityTypeConfiguration<ProductVoucherEntity>
{
    public void Configure(EntityTypeBuilder<ProductVoucherEntity> builder)
    {
        builder.ToTable(
            name: ProductVoucherEntity.MetaData.TableName,
            schema: $"{DatabaseSchemaName.MAIN}.{DatabaseSchemaName.VOUCHER}",
            buildAction: table => table.HasComment(comment: "Contain product vouchers.")
        );

        builder.HasKey(keyExpression: builder => new { builder.ProductId, builder.VoucherCode });

        builder
            .Property(propertyExpression: builder => builder.VoucherCode)
            .HasMaxLength(maxLength: ProductVoucherEntity.MetaData.VoucherCode.MaxLength);

        builder
            .Property(propertyExpression: builder => builder.ProductId)
            .HasMaxLength(maxLength: ProductVoucherEntity.MetaData.ProductId.MaxLength);

        #region Relationships
        builder
            .HasOne(navigationExpression: productVoucher => productVoucher.Voucher)
            .WithMany(navigationExpression: voucher => voucher.ProductVouchers)
            .HasForeignKey(foreignKeyExpression: productVoucher => productVoucher.VoucherCode)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);

        builder
            .HasOne(navigationExpression: productVoucher => productVoucher.Product)
            .WithMany(navigationExpression: product => product.ProductVouchers)
            .HasForeignKey(foreignKeyExpression: productVoucher => productVoucher.ProductId)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);
        #endregion
    }
}
