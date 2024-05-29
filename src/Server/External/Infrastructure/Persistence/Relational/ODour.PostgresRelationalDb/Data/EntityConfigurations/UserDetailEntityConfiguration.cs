using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.User.Entities;
using static ODour.PostgresRelationalDb.Common.CommonConstant;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class UserDetailEntityConfiguration : IEntityTypeConfiguration<UserDetailEntity>
{
    public void Configure(EntityTypeBuilder<UserDetailEntity> builder)
    {
        builder.ToTable(
            name: UserDetailEntity.MetaData.TableName,
            schema: $"{DatabaseSchemaName.MAIN}.{DatabaseSchemaName.USER}",
            buildAction: table => table.HasComment(comment: "Contain user details.")
        );

        builder.HasKey(keyExpression: builder => builder.UserId);

        builder
            .Property(propertyExpression: builder => builder.AccountStatusId)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.FirstName)
            .HasMaxLength(maxLength: UserDetailEntity.MetaData.FirstName.MaxLength)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.LastName)
            .HasMaxLength(maxLength: UserDetailEntity.MetaData.LastName.MaxLength)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.AvatarUrl)
            .HasColumnType(typeName: DatabaseNativeType.TEXT)
            .IsRequired(required: true);

        builder.Property(propertyExpression: builder => builder.Gender).IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.IsTemporarilyRemoved)
            .IsRequired(required: true);

        #region Relationships
        builder
            .HasOne(navigationExpression: userDetail => userDetail.AccountStatus)
            .WithMany(navigationExpression: accountStatus => accountStatus.UserDetails)
            .HasForeignKey(foreignKeyExpression: userDetail => userDetail.AccountStatusId)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);

        builder
            .HasOne(navigationExpression: userDetail => userDetail.User)
            .WithOne(navigationExpression: user => user.UserDetail)
            .HasForeignKey<UserDetailEntity>(foreignKeyExpression: userDetail => userDetail.UserId)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);
        #endregion
    }
}
