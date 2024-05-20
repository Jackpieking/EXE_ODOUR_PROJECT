using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.Entities;
using ODour.PostgresRelationalDb.Shared;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class AccountStatusEntityConfiguration
    : IEntityTypeConfiguration<AccountStatusEntity>
{
    public void Configure(EntityTypeBuilder<AccountStatusEntity> builder)
    {
        builder.ToTable(
            name: AccountStatusEntity.MetaData.TableName,
            buildAction: table => table.HasComment(comment: "Contain account statuses.")
        );

        builder.HasKey(keyExpression: builder => builder.Id);

        builder
            .Property(propertyExpression: builder => builder.Name)
            .HasMaxLength(maxLength: AccountStatusEntity.MetaData.Name.MaxLength)
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
            .HasOne(navigationExpression: accountStatus => accountStatus.Creator)
            .WithMany(navigationExpression: systemAccount => systemAccount.AccountStatusCreators)
            .HasForeignKey(foreignKeyExpression: accountStatus => accountStatus.CreatedBy)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);

        builder
            .HasOne(navigationExpression: accountStatus => accountStatus.Updater)
            .WithMany(navigationExpression: systemAccount => systemAccount.AccountStatusUpdaters)
            .HasForeignKey(foreignKeyExpression: accountStatus => accountStatus.UpdatedBy)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);

        builder
            .HasOne(navigationExpression: accountStatus => accountStatus.Remover)
            .WithMany(navigationExpression: systemAccount => systemAccount.AccountStatusRemovers)
            .HasForeignKey(foreignKeyExpression: accountStatus => accountStatus.RemovedBy)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);
        #endregion
    }
}
