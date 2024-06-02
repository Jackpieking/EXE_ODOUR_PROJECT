using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.User.Entities;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class UserRoleEntityConfiguration : IEntityTypeConfiguration<UserRoleEntity>
{
    public void Configure(EntityTypeBuilder<UserRoleEntity> builder)
    {
        builder.ToTable(
            name: UserRoleEntity.MetaData.TableName,
            buildAction: table => table.HasComment(comment: "Contain user roles.")
        );

        #region Relationships
        builder
            .HasOne(navigationExpression: userRole => userRole.User)
            .WithMany(navigationExpression: user => user.UserRoles)
            .HasForeignKey(foreignKeyExpression: userRole => userRole.UserId)
            .OnDelete(deleteBehavior: DeleteBehavior.Cascade);

        builder
            .HasOne(navigationExpression: userRole => userRole.Role)
            .WithMany(navigationExpression: role => role.UserRoles)
            .HasForeignKey(foreignKeyExpression: userRole => userRole.RoleId)
            .OnDelete(deleteBehavior: DeleteBehavior.Cascade);
        #endregion
    }
}
