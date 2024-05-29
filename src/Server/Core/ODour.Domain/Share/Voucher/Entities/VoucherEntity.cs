using System;
using System.Collections.Generic;
using ODour.Domain.Share.Base.Entities;

namespace ODour.Domain.Share.Voucher.Entities;

public sealed class VoucherEntity : IEntity, ITemporarilyRemovedEntity
{
    #region PrimaryKeys
    public string Code { get; set; }
    #endregion

    public string Name { get; set; }

    public int QuantityInStock { get; set; }

    public decimal DiscountPercentage { get; set; }

    public string Description { get; set; }

    public DateTime DueDate { get; set; }

    public DateTime StartDate { get; set; }

    public bool IsTemporarilyRemoved { get; set; }

    #region ForeignKeys
    public Guid VoucherTypeId { get; set; }
    #endregion

    #region NavigationProperties
    public VoucherTypeEntity VoucherType { get; set; }
    #endregion

    #region NavigationCollections
    public IEnumerable<UserVoucherEntity> UserVouchers { get; set; }

    public IEnumerable<ProductVoucherEntity> ProductVouchers { get; set; }
    #endregion

    #region MetaData
    public static class MetaData
    {
        public const string TableName = "Vouchers";

        public static class Code
        {
            public const int MaxLength = 30;

            public const int MinLength = 1;
        }

        public static class Name
        {
            public const int MaxLength = 100;

            public const int MinLength = 1;
        }

        public static class QuantityInStock
        {
            public const int MaxValue = int.MaxValue;

            public const int MinValue = default;
        }

        public static class DiscountPercentage
        {
            public const int Precision = 7;

            public const int Scale = 2;
        }

        public static class Description
        {
            public const int MinLength = 1;
        }

        public static class ProductId
        {
            public const int MaxLength = 10;

            public const int MinLength = 2;
        }
    }
    #endregion
}
