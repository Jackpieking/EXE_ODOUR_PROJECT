using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.Log.Entities;
using ODour.PostgresRelationalDb.Common;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class AppExceptionLoggingEntityConfiguration
    : IEntityTypeConfiguration<AppExceptionLoggingEntity>
{
    public void Configure(EntityTypeBuilder<AppExceptionLoggingEntity> builder)
    {
        builder.ToTable(
            name: AppExceptionLoggingEntity.MetaData.TableName,
            schema: $"{CommonConstant.DatabaseSchemaName.MAIN}.{CommonConstant.DatabaseSchemaName.APP_LOG}",
            buildAction: table => table.HasComment(comment: "Contain app exception loggings.")
        );

        builder.HasKey(keyExpression: errorLogging => errorLogging.Id);

        builder
            .Property(propertyExpression: userDetail => userDetail.ErrorMessage)
            .HasColumnType(typeName: CommonConstant.DatabaseNativeType.TEXT)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: userDetail => userDetail.ErrorStackTrace)
            .HasColumnType(typeName: CommonConstant.DatabaseNativeType.TEXT)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: userDetail => userDetail.CreatedAt)
            .HasColumnType(typeName: CommonConstant.DatabaseNativeType.TEXT)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: userDetail => userDetail.Data)
            .HasColumnType(typeName: CommonConstant.DatabaseNativeType.TEXT)
            .IsRequired(required: true);
    }
}
