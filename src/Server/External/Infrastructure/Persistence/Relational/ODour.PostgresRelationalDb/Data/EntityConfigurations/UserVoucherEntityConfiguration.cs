using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.Voucher.Entities;
using static ODour.PostgresRelationalDb.Common.CommonConstant;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class UserVoucherEntityConfiguration : IEntityTypeConfiguration<UserVoucherEntity>
{
    public void Configure(EntityTypeBuilder<UserVoucherEntity> builder)
    {
        builder.ToTable(
            name: UserVoucherEntity.MetaData.TableName,
            schema: $"{DatabaseSchemaName.MAIN}.{DatabaseSchemaName.VOUCHER}",
            buildAction: table => table.HasComment(comment: "Contain user vouchers.")
        );

        builder.HasKey(keyExpression: builder => new { builder.UserId, builder.VoucherCode });

        builder
            .Property(propertyExpression: builder => builder.VoucherCode)
            .HasMaxLength(maxLength: UserVoucherEntity.MetaData.VoucherCode.MaxLength);

        #region Relationships
        builder
            .HasOne(navigationExpression: userVoucher => userVoucher.UserDetail)
            .WithMany(navigationExpression: userDetail => userDetail.UserVouchers)
            .HasForeignKey(foreignKeyExpression: userVoucher => userVoucher.UserId)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);

        builder
            .HasOne(navigationExpression: userVoucher => userVoucher.Voucher)
            .WithMany(navigationExpression: voucher => voucher.UserVouchers)
            .HasForeignKey(foreignKeyExpression: userVoucher => userVoucher.VoucherCode)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);
        #endregion
    }
}
