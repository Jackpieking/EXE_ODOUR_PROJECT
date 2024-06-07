using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.System.Entities;
using static ODour.PostgresRelationalDb.Common.CommonConstant;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class AppExceptionLoggingEntityConfiguration
    : IEntityTypeConfiguration<AppExceptionLoggingEntity>
{
    public void Configure(EntityTypeBuilder<AppExceptionLoggingEntity> builder)
    {
        builder.ToTable(
            name: AppExceptionLoggingEntity.MetaData.TableName,
            schema: $"{DatabaseSchemaName.MAIN}.{DatabaseSchemaName.SYSTEM}",
            buildAction: table => table.HasComment(comment: "Contain app exception loggings.")
        );

        builder.HasKey(keyExpression: entity => entity.Id);

        builder
            .Property(propertyExpression: entity => entity.ErrorMessage)
            .HasColumnType(typeName: DatabaseNativeType.TEXT)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: entity => entity.ErrorStackTrace)
            .HasColumnType(typeName: DatabaseNativeType.TEXT)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: entity => entity.CreatedAt)
            .HasColumnType(typeName: DatabaseNativeType.TEXT)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: entity => entity.Data)
            .HasColumnType(typeName: DatabaseNativeType.TEXT)
            .IsRequired(required: true);
    }
}
