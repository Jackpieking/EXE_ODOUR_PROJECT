using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.System.Entities;
using static ODour.PostgresRelationalDb.Common.CommonConstant;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations
{
    internal sealed class JobRecordEntityConfiguration : IEntityTypeConfiguration<JobRecordEntity>
    {
        public void Configure(EntityTypeBuilder<JobRecordEntity> builder)
        {
            builder.ToTable(
                name: JobRecordEntity.MetaData.TableName,
                schema: $"{DatabaseSchemaName.MAIN}.{DatabaseSchemaName.SYSTEM}",
                buildAction: table => table.HasComment(comment: "Contain job records.")
            );

            builder.HasKey(keyExpression: errorLogging => errorLogging.Id);

            builder
                .Property(propertyExpression: userDetail => userDetail.QueueID)
                .HasColumnType(typeName: DatabaseNativeType.TEXT)
                .IsRequired(required: true);

            builder
                .Property(propertyExpression: userDetail => userDetail.ExecuteAfter)
                .HasColumnType(typeName: DatabaseNativeType.TIMESTAMPTZ)
                .IsRequired(required: true);

            builder
                .Property(propertyExpression: userDetail => userDetail.ExpireOn)
                .HasColumnType(typeName: DatabaseNativeType.TIMESTAMPTZ)
                .IsRequired(required: true);

            builder
                .Property(propertyExpression: userDetail => userDetail.IsComplete)
                .IsRequired(required: true);

            builder
                .Property(propertyExpression: userDetail => userDetail.CommandJson)
                .HasColumnType(typeName: DatabaseNativeType.TEXT)
                .IsRequired(required: true);
        }
    }
}
