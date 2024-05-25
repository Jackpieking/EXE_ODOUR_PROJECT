using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.AccountStatus.Entities;
using ODour.PostgresRelationalDb.Common;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class AccountStatusEventEntityConfiguration
    : IEntityTypeConfiguration<AccountStatusEventEntity>
{
    public void Configure(EntityTypeBuilder<AccountStatusEventEntity> builder)
    {
        builder.ToTable(
            name: AccountStatusEventEntity.MetaData.TableName,
            schema: $"{CommonConstant.DatabaseSchemaName.MAIN}.{CommonConstant.DatabaseSchemaName.ACCOUNT_STATUS}",
            buildAction: table => table.HasComment(comment: "Contain account status events.")
        );

        builder.HasKey(keyExpression: builder => builder.Id);

        builder
            .Property(propertyExpression: builder => builder.Type)
            .HasMaxLength(maxLength: AccountStatusEventEntity.MetaData.Type.MaxLength)
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
            .HasOne(navigationExpression: accountStatusEvent => accountStatusEvent.Creator)
            .WithMany(navigationExpression: systemAccount =>
                systemAccount.AccountStatusEventCreators
            )
            .HasForeignKey(foreignKeyExpression: accountStatusEvent => accountStatusEvent.CreatedBy)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);
        #endregion
    }
}
