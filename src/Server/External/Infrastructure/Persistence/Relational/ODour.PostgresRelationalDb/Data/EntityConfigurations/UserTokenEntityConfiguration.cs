using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.User.Entities;
using ODour.PostgresRelationalDb.Common;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class UserTokenEntityConfiguration : IEntityTypeConfiguration<UserTokenEntity>
{
    public void Configure(EntityTypeBuilder<UserTokenEntity> builder)
    {
        builder.ToTable(
            name: UserTokenEntity.MetaData.TableName,
            buildAction: table => table.HasComment(comment: "Contain user tokens.")
        );

        builder
            .Property(propertyExpression: builder => builder.ExpiredAt)
            .HasColumnType(typeName: CommonConstant.DatabaseNativeType.TIMESTAMPTZ)
            .IsRequired(required: true);

        #region Relationships
        builder
            .HasOne(navigationExpression: userToken => userToken.User)
            .WithMany(navigationExpression: user => user.UserTokens)
            .HasForeignKey(foreignKeyExpression: userToken => userToken.UserId)
            .OnDelete(deleteBehavior: DeleteBehavior.Cascade);
        #endregion
    }
}
