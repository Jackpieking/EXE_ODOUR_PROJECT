using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.Entities;
using ODour.PostgresRelationalDb.Shared;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class CategoryEntityConfiguration : IEntityTypeConfiguration<CategoryEntity>
{
    public void Configure(EntityTypeBuilder<CategoryEntity> builder)
    {
        builder.ToTable(
            name: CategoryEntity.MetaData.TableName,
            buildAction: table => table.HasComment(comment: "Contain categories.")
        );

        builder.HasKey(keyExpression: builder => builder.Id);

        builder
            .Property(propertyExpression: builder => builder.Name)
            .HasMaxLength(maxLength: CategoryEntity.MetaData.Name.MaxLength)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.CreatedAt)
            .HasColumnType(typeName: CommonConstant.DatabaseNativeType.TIMESTAMPTZ)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.CreatedBy)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.UpdatedAt)
            .HasColumnType(typeName: CommonConstant.DatabaseNativeType.TIMESTAMPTZ)
            .IsRequired(required: true);

        builder
            .Property(propertyExpression: builder => builder.UpdatedBy)
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
            .HasOne(navigationExpression: category => category.Creator)
            .WithMany(navigationExpression: systemAccount => systemAccount.CategoryCreators)
            .HasForeignKey(foreignKeyExpression: category => category.CreatedBy)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);

        builder
            .HasOne(navigationExpression: category => category.Updater)
            .WithMany(navigationExpression: systemAccount => systemAccount.CategoryUpdaters)
            .HasForeignKey(foreignKeyExpression: category => category.UpdatedBy)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);

        builder
            .HasOne(navigationExpression: category => category.Remover)
            .WithMany(navigationExpression: systemAccount => systemAccount.CategoryRemovers)
            .HasForeignKey(foreignKeyExpression: category => category.RemovedBy)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);
        #endregion
    }
}
