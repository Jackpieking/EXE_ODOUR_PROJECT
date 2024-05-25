using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.Role.Entities;
using ODour.PostgresRelationalDb.Common;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class RoleEventEntityConfiguration : IEntityTypeConfiguration<RoleEventEntity>
{
    public void Configure(EntityTypeBuilder<RoleEventEntity> builder)
    {
        builder.ToTable(
            name: RoleEventEntity.MetaData.TableName,
            schema: $"{CommonConstant.DatabaseSchemaName.MAIN}.{CommonConstant.DatabaseSchemaName.ROLE}",
            buildAction: table => table.HasComment(comment: "Contain role events.")
        );

        builder.HasKey(keyExpression: builder => builder.Id);

        builder
            .Property(propertyExpression: builder => builder.Type)
            .HasMaxLength(maxLength: RoleEventEntity.MetaData.Type.MaxLength)
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
            .HasOne(navigationExpression: roleEvent => roleEvent.Creator)
            .WithMany(navigationExpression: systemAccount => systemAccount.RoleEventCreators)
            .HasForeignKey(foreignKeyExpression: roleEvent => roleEvent.CreatedBy)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);
        #endregion
    }
}
