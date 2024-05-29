using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.Base.Events;
using static ODour.PostgresRelationalDb.Common.CommonConstant;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class EventSnapshotEntityConfiguration
    : IEntityTypeConfiguration<EventSnapshotEntity>
{
    public void Configure(EntityTypeBuilder<EventSnapshotEntity> builder)
    {
        builder.ToTable(
            name: EventSnapshotEntity.MetaData.TableName,
            schema: $"{DatabaseSchemaName.MAIN}.{DatabaseSchemaName.EVENT}",
            buildAction: table => table.HasComment(comment: "Contain event snapshots.")
        );

        builder.HasKey(keyExpression: builder => builder.EventId);

        builder
            .Property(propertyExpression: builder => builder.IsCompleted)
            .IsRequired(required: true);

        builder
            .HasOne(navigationExpression: eventSnapshot => eventSnapshot.Event)
            .WithOne(navigationExpression: myEvent => myEvent.EventSnapshot)
            .HasForeignKey<EventSnapshotEntity>(foreignKeyExpression: eventSnapshot =>
                eventSnapshot.EventId
            )
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);
    }
}
