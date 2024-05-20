using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.Entities;
using ODour.PostgresRelationalDb.Shared;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class SystemAccountTokenEntityConfiguration
    : IEntityTypeConfiguration<SystemAccountTokenEntity>
{
    public void Configure(EntityTypeBuilder<SystemAccountTokenEntity> builder)
    {
        builder.ToTable(
            name: SystemAccountTokenEntity.MetaData.TableName,
            buildAction: table => table.HasComment(comment: "Contain system account tokens.")
        );

        builder.HasKey(keyExpression: builder => builder.SystemAccountId);

        builder
            .Property(propertyExpression: builder => builder.Name)
            .HasMaxLength(maxLength: SystemAccountTokenEntity.MetaData.Name.MaxLength)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.Value)
            .HasColumnType(typeName: CommonConstant.DatabaseNativeType.TEXT)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.CreatedAt)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.ExpiredAt)
            .IsRequired(required: true);

        #region Relationships
        builder
            .HasOne(navigationExpression: systemAccountToken => systemAccountToken.SystemAccount)
            .WithMany(navigationExpression: systemAccount => systemAccount.SystemAccountTokens)
            .HasForeignKey(foreignKeyExpression: systemAccountToken =>
                systemAccountToken.SystemAccountId
            )
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);
        #endregion
    }
}
