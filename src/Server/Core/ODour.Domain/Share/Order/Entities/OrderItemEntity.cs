using System;
using ODour.Domain.Share.Base.Entities;
using ODour.Domain.Share.Product.Entities;

namespace ODour.Domain.Share.Order.Entities;

public sealed class OrderItemEntity : IEntity
{
    #region PrimaryForeignKeys
    public Guid OrderId { get; set; }

    public string ProductId { get; set; }
    #endregion

    public decimal SellingPrice { get; set; }

    public int SellingQuantity { get; set; }

    #region NavigationProperties
    public OrderEntity Order { get; set; }

    public ProductEntity Product { get; set; }
    #endregion

    #region MetaData
    public static class MetaData
    {
        public const string TableName = "OrderItems";

        public static class ProductId
        {
            public const int MinLength = 2;

            public const int MaxLength = 10;
        }

        public static class SellingPrice
        {
            public const int Precision = 12;

            public const int Scale = 2;
        }

        public static class SellingQuantity
        {
            public const int MinValue = 0;

            public const int MaxValue = int.MaxValue;
        }
    }
    #endregion
}
