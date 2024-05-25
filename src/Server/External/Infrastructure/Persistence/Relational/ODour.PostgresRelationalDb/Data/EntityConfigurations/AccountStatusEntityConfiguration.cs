using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.AccountStatus.Entities;
using ODour.PostgresRelationalDb.Common;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class AccountStatusEntityConfiguration
    : IEntityTypeConfiguration<AccountStatusEntity>
{
    public void Configure(EntityTypeBuilder<AccountStatusEntity> builder)
    {
        builder.ToTable(
            name: AccountStatusEntity.MetaData.TableName,
            schema: $"{CommonConstant.DatabaseSchemaName.MAIN}.{CommonConstant.DatabaseSchemaName.ACCOUNT_STATUS}",
            buildAction: table => table.HasComment(comment: "Contain account statuses.")
        );

        builder.HasKey(keyExpression: builder => builder.Id);

        builder
            .Property(propertyExpression: builder => builder.Name)
            .HasMaxLength(maxLength: AccountStatusEntity.MetaData.Name.MaxLength)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.IsTemporarilyRemoved)
            .IsRequired(required: true);
    }
}
