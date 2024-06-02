using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.Role.Entities;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class RoleEntityConfiguration : IEntityTypeConfiguration<RoleEntity>
{
    public void Configure(EntityTypeBuilder<RoleEntity> builder)
    {
        builder.ToTable(
            name: RoleEntity.MetaData.TableName,
            buildAction: table => table.HasComment(comment: "Contain roles.")
        );
    }
}
