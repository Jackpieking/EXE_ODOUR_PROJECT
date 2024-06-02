using System;
using System.Collections.Generic;
using ODour.Domain.Share.Base.Entities;
using ODour.Domain.Share.Product.Entities;

namespace ODour.Domain.Share.Category.Entities;

public sealed class CategoryEntity : IEntity, ITemporarilyRemovedEntity
{
    #region PrimaryKeys
    public Guid Id { get; set; }
    #endregion

    public string Name { get; set; }

    public bool IsTemporarilyRemoved { get; set; }

    #region NavigationCollections
    public IEnumerable<ProductEntity> Products { get; set; }
    #endregion

    #region MetaData
    public static class MetaData
    {
        public const string TableName = "Categories";

        public static class Name
        {
            public const int MaxLength = 50;

            public const int MinLength = 2;
        }
    }
    #endregion
}
