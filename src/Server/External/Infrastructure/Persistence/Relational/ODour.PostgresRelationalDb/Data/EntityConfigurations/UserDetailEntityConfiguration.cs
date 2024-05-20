using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.Entities;
using ODour.PostgresRelationalDb.Shared;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class UserDetailEntityConfiguration : IEntityTypeConfiguration<UserDetailEntity>
{
    public void Configure(EntityTypeBuilder<UserDetailEntity> builder)
    {
        builder.ToTable(
            name: UserDetailEntity.MetaData.TableName,
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
            .HasColumnType(typeName: CommonConstant.DatabaseNativeType.TEXT)
            .IsRequired(required: true);

        builder.Property(propertyExpression: builder => builder.Gender).IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.CreatedAt)
            .HasColumnType(typeName: CommonConstant.DatabaseNativeType.TIMESTAMPTZ)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.UpdatedAt)
            .HasColumnType(typeName: CommonConstant.DatabaseNativeType.TIMESTAMPTZ)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.RemovedAt)
            .HasColumnType(typeName: CommonConstant.DatabaseNativeType.TIMESTAMPTZ)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.RemovedBy)
            .IsRequired(required: true);

        #region Relationships
        builder
            .HasOne(navigationExpression: userDetail => userDetail.Remover)
            .WithMany(navigationExpression: systemAccount => systemAccount.UserDetailRemovers)
            .HasForeignKey(foreignKeyExpression: userDetail => userDetail.RemovedBy)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);

        builder
            .HasOne(navigationExpression: userDetail => userDetail.AccountStatus)
            .WithMany(navigationExpression: accountStatus => accountStatus.UserDetails)
            .HasForeignKey(foreignKeyExpression: userDetail => userDetail.AccountStatusId)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);

        builder
            .HasOne(navigationExpression: userDetail => userDetail.User)
            .WithOne(navigationExpression: user => user.UserDetail)
            .HasForeignKey<UserDetailEntity>(foreignKeyExpression: userDetail => userDetail.UserId)
            .OnDelete(deleteBehavior: DeleteBehavior.Cascade);
        #endregion
    }
}
