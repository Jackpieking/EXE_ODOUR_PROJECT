using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.SeedFlag.Entities;
using static ODour.PostgresRelationalDb.Common.CommonConstant;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class SeedFlagEntityConfiguration : IEntityTypeConfiguration<SeedFlagEntity>
{
    public void Configure(EntityTypeBuilder<SeedFlagEntity> builder)
    {
        builder.ToTable(
            name: SeedFlagEntity.MetaData.TableName,
            schema: $"{DatabaseSchemaName.MAIN}.{DatabaseSchemaName.SEED_FLAG}",
            buildAction: table => table.HasComment(comment: "Contain seed flags.")
        );

        builder.HasKey(keyExpression: builder => builder.Id);
    }
}
