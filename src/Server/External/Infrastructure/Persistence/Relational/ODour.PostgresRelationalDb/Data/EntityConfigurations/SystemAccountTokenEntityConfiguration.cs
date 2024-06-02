using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.System.Entities;
using static ODour.PostgresRelationalDb.Common.CommonConstant;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class SystemAccountTokenEntityConfiguration
    : IEntityTypeConfiguration<SystemAccountTokenEntity>
{
    public void Configure(EntityTypeBuilder<SystemAccountTokenEntity> builder)
    {
        builder.ToTable(
            name: SystemAccountTokenEntity.MetaData.TableName,
            schema: $"{DatabaseSchemaName.MAIN}.{DatabaseSchemaName.SYSTEM}",
            buildAction: table => table.HasComment(comment: "Contain system account tokens.")
        );

        builder.HasKey(keyExpression: builder => new
        {
            builder.SystemAccountId,
            builder.LoginProvider,
            builder.Name
        });

        builder
            .Property(propertyExpression: builder => builder.LoginProvider)
            .HasColumnType(typeName: DatabaseNativeType.TEXT)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.Name)
            .HasMaxLength(maxLength: SystemAccountTokenEntity.MetaData.Name.MaxLength)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.Value)
            .HasColumnType(typeName: DatabaseNativeType.TEXT)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.ExpiredAt)
            .HasColumnType(typeName: DatabaseNativeType.TIMESTAMPTZ)
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
