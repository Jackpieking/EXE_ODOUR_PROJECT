using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.User.Entities;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class UserClaimEntityConfiguration : IEntityTypeConfiguration<UserClaimEntity>
{
    public void Configure(EntityTypeBuilder<UserClaimEntity> builder)
    {
        builder.ToTable(
            name: UserClaimEntity.MetaData.TableName,
            buildAction: table => table.HasComment(comment: "Contain user claims.")
        );

        #region Relationships
        builder
            .HasOne(navigationExpression: userClaim => userClaim.User)
            .WithMany(navigationExpression: user => user.UserClaims)
            .OnDelete(deleteBehavior: DeleteBehavior.Cascade);
        #endregion
    }
}
