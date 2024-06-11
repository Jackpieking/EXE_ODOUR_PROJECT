using System;
using ODour.Domain.Share.Base.Entities;
using ODour.Domain.Share.Product.Entities;
using ODour.Domain.Share.User.Entities;

namespace ODour.Domain.Share.Cart.Entities;

public sealed class CartItemEntity : IEntity
{
    #region PrimaryForeignKeys
    public string ProductId { get; set; }

    public Guid UserId { get; set; }
    #endregion

    public int Quantity { get; set; }

    #region NavigationProperties
    public UserDetailEntity User { get; set; }

    public ProductEntity Product { get; set; }
    #endregion

    #region MetaData
    public static class MetaData
    {
        public const string TableName = "CartItems";

        public static class ProductId
        {
            public const int MaxLength = 10;

            public const int MinLength = 2;
        }

        public static class Quantity
        {
            public const int MaxValue = 200;

            public const int MinValue = default;
        }
    }
    #endregion
}
