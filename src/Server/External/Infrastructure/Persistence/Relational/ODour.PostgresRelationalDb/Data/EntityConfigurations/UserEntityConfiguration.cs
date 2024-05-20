using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.Entities;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable(
            name: UserEntity.MetaData.TableName,
            buildAction: table => table.HasComment(comment: "Contain users.")
        );
    }
}
