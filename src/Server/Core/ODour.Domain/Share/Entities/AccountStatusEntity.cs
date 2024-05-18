using System;
using System.Collections.Generic;
using ODour.Domain.Share.Entities.Base;

namespace ODour.Domain.Share.Entities;

public sealed class AccountStatusEntity
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

    #region NavigationCollections
    public IEnumerable<UserEntity> UserEntities { get; set; }
    #endregion

    #region MetaData
    public static class MetaData
    {
        public const string TableName = "AccountStatuses";

        public static class Name
        {
            public const int MaxLength = 50;

            public const int MinLength = 2;
        }
    }
    #endregion
}
