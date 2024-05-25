using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.User.Entities;
using ODour.PostgresRelationalDb.Common;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class UserEventEntityConfiguration : IEntityTypeConfiguration<UserEventEntity>
{
    public void Configure(EntityTypeBuilder<UserEventEntity> builder)
    {
        builder.ToTable(
            name: UserEventEntity.MetaData.TableName,
            schema: $"{CommonConstant.DatabaseSchemaName.MAIN}.{CommonConstant.DatabaseSchemaName.USER}",
            buildAction: table => table.HasComment(comment: "Contain user events.")
        );

        builder.HasKey(keyExpression: builder => builder.Id);

        builder
            .Property(propertyExpression: builder => builder.Type)
            .HasMaxLength(maxLength: UserEventEntity.MetaData.Type.MaxLength)
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
    }
}
