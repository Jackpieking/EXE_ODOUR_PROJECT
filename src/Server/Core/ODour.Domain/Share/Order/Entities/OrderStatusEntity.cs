using System;
using System.Collections.Generic;
using ODour.Domain.Share.Base.Entities;

namespace ODour.Domain.Share.Order.Entities;

public sealed class OrderStatusEntity : IEntity, ITemporarilyRemovedEntity
{
    #region PrimaryKeys
    public Guid Id { get; set; }
    #endregion

    public string Name { get; set; }

    public bool IsTemporarilyRemoved { get; set; }

    #region NavigationCollections
    public IEnumerable<OrderEntity> Orders { get; set; }
    #endregion

    #region MetaData
    public static class MetaData
    {
        public const string TableName = "OrderStatuses";

        public static class Name
        {
            public const int MaxLength = 50;

            public const int MinLength = 2;
        }
    }
    #endregion
}
