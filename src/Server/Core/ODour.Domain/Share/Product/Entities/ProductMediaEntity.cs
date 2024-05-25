using System;
using ODour.Domain.Share.Base.Entities;

namespace ODour.Domain.Share.Product.Entities;

public sealed class ProductMediaEntity : IEntity
{
    #region PrimaryKeys
    public Guid Id { get; set; }
    #endregion

    #region ForeignKeys
    public string ProductId { get; set; }
    #endregion

    public int UploadOrder { get; set; }

    public string StorageUrl { get; set; }

    #region NavigationProperties
    public ProductEntity Product { get; set; }
    #endregion

    #region MetaData
    public static class MetaData
    {
        public const string TableName = "ProductMedias";

        public static class ProductId
        {
            public const int MinLength = 2;

            public const int MaxLength = 10;
        }

        public static class UploadOrder
        {
            public const int MinValue = 0;

            public const int MaxValue = int.MaxValue;
        }

        public static class StorageUrl
        {
            public const int MinLength = 2;
        }
    }
    #endregion
}
