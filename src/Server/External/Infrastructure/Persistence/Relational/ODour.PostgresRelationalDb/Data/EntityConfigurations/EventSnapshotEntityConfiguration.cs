using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.EventSnapshot.Entities;
using ODour.PostgresRelationalDb.Common;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class EventSnapshotEntityConfiguration
    : IEntityTypeConfiguration<EventSnapshotEntity>
{
    public void Configure(EntityTypeBuilder<EventSnapshotEntity> builder)
    {
        builder.ToTable(
            name: EventSnapshotEntity.MetaData.TableName,
            schema: $"{CommonConstant.DatabaseSchemaName.MAIN}.{CommonConstant.DatabaseSchemaName.EVENT_SNAPSHOT}",
            buildAction: table => table.HasComment(comment: "Contain event snapshots.")
        );

        builder.HasKey(keyExpression: builder => new { builder.EventId, builder.StreamId });

        builder
            .Property(propertyExpression: builder => builder.StreamId)
            .HasColumnType(typeName: CommonConstant.DatabaseNativeType.TEXT)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.IsCompleted)
            .IsRequired(required: true);
    }
}
