using System;
using System.Collections.Generic;
using ODour.Domain.Share.Base.Entities;
using ODour.Domain.Share.Order.Entities;

namespace ODour.Domain.Share.Payment.Entities;

public sealed class PaymentMethodEntity : IEntity, ITemporarilyRemovedEntity
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
        public const string TableName = "PaymentMethods";

        public static class Name
        {
            public const int MinLength = 2;

            public const int MaxLength = 100;
        }
    }
    #endregion
}
