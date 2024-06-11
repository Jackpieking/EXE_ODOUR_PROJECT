using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ODour.Domain.Share.Cart.Entities;
using static ODour.PostgresRelationalDb.Common.CommonConstant;

namespace ODour.PostgresRelationalDb.Data.EntityConfigurations;

internal sealed class CartItemEntityConfiguration : IEntityTypeConfiguration<CartItemEntity>
{
    public void Configure(EntityTypeBuilder<CartItemEntity> builder)
    {
        builder.ToTable(
            name: CartItemEntity.MetaData.TableName,
            schema: $"{DatabaseSchemaName.MAIN}.{DatabaseSchemaName.ORDER}",
            buildAction: table => table.HasComment(comment: "Contain cart items.")
        );

        builder.HasKey(keyExpression: entity => new { entity.ProductId, entity.UserId });

        builder.Property(propertyExpression: entity => entity.Quantity).IsRequired();

        #region Relationships
        builder
            .HasOne(navigationExpression: cartItem => cartItem.Product)
            .WithMany(navigationExpression: product => product.CartItems)
            .HasForeignKey(foreignKeyExpression: cartItem => cartItem.ProductId)
            .OnDelete(deleteBehavior: DeleteBehavior.Cascade);

        builder
            .HasOne(navigationExpression: cartItem => cartItem.User)
            .WithMany(navigationExpression: user => user.CartItems)
            .HasForeignKey(foreignKeyExpression: cartItem => cartItem.UserId)
            .OnDelete(deleteBehavior: DeleteBehavior.Cascade);
        #endregion
    }
}
