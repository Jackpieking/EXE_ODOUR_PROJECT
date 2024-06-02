using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.User.Entities;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class UserLoginEntityConfiguration : IEntityTypeConfiguration<UserLoginEntity>
{
    public void Configure(EntityTypeBuilder<UserLoginEntity> builder)
    {
        builder.ToTable(
            name: UserLoginEntity.MetaData.TableName,
            buildAction: table => table.HasComment(comment: "Contain user logins.")
        );

        #region Relationships
        builder
            .HasOne(navigationExpression: userLogin => userLogin.User)
            .WithMany(navigationExpression: user => user.UserLogins)
            .HasForeignKey(foreignKeyExpression: userLogin => userLogin.UserId)
            .OnDelete(deleteBehavior: DeleteBehavior.Cascade);
        #endregion
    }
}
