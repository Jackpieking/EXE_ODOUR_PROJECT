using ODour.Domain.Share.Base.Entities;
using ODour.Domain.Share.Product.Entities;

namespace ODour.Domain.Share.Voucher.Entities;

public sealed class ProductVoucherEntity : IEntity
{
    #region PrimaryForeignKeys
    public string ProductId { get; set; }

    public string VoucherCode { get; set; }
    #endregion

    #region NavigationProperties
    public ProductEntity Product { get; set; }

    public VoucherEntity Voucher { get; set; }
    #endregion

    #region MetaData
    public static class MetaData
    {
        public const string TableName = "ProductVouchers";

        public static class VoucherCode
        {
            public const int MaxLength = 30;

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
