using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.Role.Entities;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class RoleClaimEntityConfiguration : IEntityTypeConfiguration<RoleClaimEntity>
{
    public void Configure(EntityTypeBuilder<RoleClaimEntity> builder)
    {
        builder.ToTable(
            name: RoleClaimEntity.MetaData.TableName,
            buildAction: table => table.HasComment(comment: "Contain role claims.")
        );

        #region Relationships
        builder
            .HasOne(navigationExpression: roleClaim => roleClaim.Role)
            .WithMany(navigationExpression: role => role.RoleClaims)
            .HasForeignKey(foreignKeyExpression: roleClaim => roleClaim.RoleId)
            .OnDelete(deleteBehavior: DeleteBehavior.Cascade);
        #endregion
    }
}
