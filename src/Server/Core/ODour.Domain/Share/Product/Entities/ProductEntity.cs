using System;
using System.Collections.Generic;
using ODour.Domain.Share.Base.Entities;
using ODour.Domain.Share.Category.Entities;
using ODour.Domain.Share.Order.Entities;
using ODour.Domain.Share.Voucher.Entities;

namespace ODour.Domain.Share.Product.Entities;

public sealed class ProductEntity : IEntity, ITemporarilyRemovedEntity
{
    #region PrimaryKeys
    public string Id { get; set; }
    #endregion

    #region ForeignKeys
    public Guid ProductStatusId { get; set; }

    public Guid CategoryId { get; set; }
    #endregion

    public string Name { get; set; }

    public decimal UnitPrice { get; set; }

    public string Description { get; set; }

    public int QuantityInStock { get; set; }

    public bool IsTemporarilyRemoved { get; set; }

    #region NavigationProperties
    public CategoryEntity Category { get; set; }

    public ProductStatusEntity ProductStatus { get; set; }
    #endregion

    #region NavigationCollections
    public IEnumerable<ProductMediaEntity> ProductImages { get; set; }

    public IEnumerable<OrderItemEntity> OrderItems { get; set; }

    public IEnumerable<ProductVoucherEntity> ProductVouchers { get; set; }
    #endregion

    #region MetaData
    public static class MetaData
    {
        public const string TableName = "Products";

        public static class Id
        {
            public const int MaxLength = 10;

            public const int MinLength = 2;
        }

        public static class Name
        {
            public const int MaxLength = 200;

            public const int MinLength = 2;
        }

        public static class UnitPrice
        {
            public const int Precision = 12;

            public const int Scale = 2;
        }

        public static class Description
        {
            public const int MinLength = 2;
        }

        public static class QuantityInStock
        {
            public const int MinValue = 0;

            public const int MaxValue = int.MaxValue;
        }
    }
    #endregion
}
