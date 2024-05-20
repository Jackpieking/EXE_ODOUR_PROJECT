using System;
using ODour.Domain.Share.Entities.Base;

namespace ODour.Domain.Share.Entities;

public sealed class ProductImageEntity : IEntity
{
    #region PrimaryKeys
    public Guid Id { get; set; }
    #endregion

    #region ForeignKeys
    public Guid ProductId { get; set; }
    #endregion

    public int UploadOrder { get; set; }

    public string StorageUrl { get; set; }

    #region NavigationProperties
    public ProductEntity Product { get; set; }
    #endregion

    #region MetaData
    public static class MetaData
    {
        public const string TableName = "ProductImages";

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
