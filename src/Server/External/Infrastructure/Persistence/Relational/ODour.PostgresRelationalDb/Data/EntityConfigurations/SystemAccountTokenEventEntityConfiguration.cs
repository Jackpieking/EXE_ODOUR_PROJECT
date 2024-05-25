using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.SystemAccount.Entities;
using ODour.PostgresRelationalDb.Common;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class SystemAccountTokenEventEntityConfiguration
    : IEntityTypeConfiguration<SystemAccountTokenEventEntity>
{
    public void Configure(EntityTypeBuilder<SystemAccountTokenEventEntity> builder)
    {
        builder.ToTable(
            name: SystemAccountTokenEventEntity.MetaData.TableName,
            schema: $"{CommonConstant.DatabaseSchemaName.MAIN}.{CommonConstant.DatabaseSchemaName.SYSTEM_ACCOUNT}",
            buildAction: table => table.HasComment(comment: "Contain system account token events.")
        );

        builder.HasKey(keyExpression: builder => builder.Id);

        builder
            .Property(propertyExpression: builder => builder.Type)
            .HasMaxLength(maxLength: SystemAccountTokenEventEntity.MetaData.Type.MaxLength)
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
            .HasOne(navigationExpression: systemAccountTokenEvent =>
                systemAccountTokenEvent.Creator
            )
            .WithMany(navigationExpression: systemAccount =>
                systemAccount.SystemAccountTokenEventCreators
            )
            .HasForeignKey(foreignKeyExpression: systemAccountTokenEvent =>
                systemAccountTokenEvent.CreatedBy
            )
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);
        #endregion
    }
}
