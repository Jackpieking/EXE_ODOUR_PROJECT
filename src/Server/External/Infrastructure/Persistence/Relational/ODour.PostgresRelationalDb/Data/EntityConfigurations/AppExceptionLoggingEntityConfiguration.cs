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

        builder.HasKey(keyExpression: errorLogging => errorLogging.Id);

        builder
            .Property(propertyExpression: userDetail => userDetail.ErrorMessage)
            .HasColumnType(typeName: DatabaseNativeType.TEXT)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: userDetail => userDetail.ErrorStackTrace)
            .HasColumnType(typeName: DatabaseNativeType.TEXT)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: userDetail => userDetail.CreatedAt)
            .HasColumnType(typeName: DatabaseNativeType.TEXT)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: userDetail => userDetail.Data)
            .HasColumnType(typeName: DatabaseNativeType.TEXT)
            .IsRequired(required: true);
    }
}
