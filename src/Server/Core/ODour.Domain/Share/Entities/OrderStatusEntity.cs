using System;
using System.Collections.Generic;
using ODour.Domain.Share.Entities.Base;

namespace ODour.Domain.Share.Entities;

public sealed class OrderStatusEntity
    : IEntity,
        ICreatedEntity,
        IUpdatedEntity,
        ITemporarilyRemovedEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Guid UpdatedBy { get; set; }

    public DateTime RemovedAt { get; set; }

    public Guid RemovedBy { get; set; }

    #region NavigationProperties
    public SystemAccountEntity Creator { get; set; }

    public SystemAccountEntity Updater { get; set; }

    public SystemAccountEntity Remover { get; set; }
    #endregion

    #region NavigationCollections
    public IEnumerable<OrderEntity> OrderEntities { get; set; }
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
