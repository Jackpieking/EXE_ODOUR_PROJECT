using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.System.Entities;
using static ODour.PostgresRelationalDb.Common.CommonConstant;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class SystemAccountEntityConfiguration
    : IEntityTypeConfiguration<SystemAccountEntity>
{
    public void Configure(EntityTypeBuilder<SystemAccountEntity> builder)
    {
        builder.ToTable(
            name: SystemAccountEntity.MetaData.TableName,
            schema: $"{DatabaseSchemaName.MAIN}.{DatabaseSchemaName.SYSTEM}",
            buildAction: table => table.HasComment(comment: "Contain system accounts.")
        );

        builder.HasKey(keyExpression: builder => builder.Id);

        builder
            .Property(propertyExpression: builder => builder.AccountStatusId)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.UserName)
            .HasMaxLength(maxLength: SystemAccountEntity.MetaData.UserName.MaxLength)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.NormalizedUserName)
            .HasMaxLength(maxLength: SystemAccountEntity.MetaData.NormalizedUserName.MaxLength)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.Email)
            .HasMaxLength(maxLength: SystemAccountEntity.MetaData.Email.MaxLength)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.NormalizedEmail)
            .HasMaxLength(maxLength: SystemAccountEntity.MetaData.NormalizedEmail.MaxLength)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.PasswordHash)
            .HasColumnType(typeName: DatabaseNativeType.TEXT)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.AccessFailedCount)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.EmailConfirmed)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.LockoutEnd)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.IsTemporarilyRemoved)
            .IsRequired(required: true);

        #region Relationships
        builder
            .HasOne(navigationExpression: systemAccount => systemAccount.AccountStatus)
            .WithMany(navigationExpression: accountStatus => accountStatus.SystemAccounts)
            .HasForeignKey(foreignKeyExpression: systemAccount => systemAccount.AccountStatusId)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);
        #endregion

        #region Indexes
        builder
            .HasIndex(
                indexExpression: systemAccount => systemAccount.NormalizedUserName,
                name: "IX_SystemAccount_NormalizedUserName"
            )
            .IsUnique(unique: true);

        builder
            .HasIndex(
                indexExpression: systemAccount => systemAccount.NormalizedEmail,
                name: "IX_SystemAccount_NormalizedEmail"
            )
            .IsUnique(unique: true);
        #endregion
    }
}
