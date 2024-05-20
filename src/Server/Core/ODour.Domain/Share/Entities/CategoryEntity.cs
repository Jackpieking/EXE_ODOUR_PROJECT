using System;
using System.Collections.Generic;
using ODour.Domain.Share.Entities.Base;

namespace ODour.Domain.Share.Entities;

public sealed class CategoryEntity
    : IEntity,
        ICreatedEntity,
        IUpdatedEntity,
        ITemporarilyRemovedEntity
{
    #region PrimaryKeys
    public Guid Id { get; set; }
    #endregion

    public string Name { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime RemovedAt { get; set; }

    #region ForeignKeys
    public Guid CreatedBy { get; set; }

    public Guid UpdatedBy { get; set; }

    public Guid RemovedBy { get; set; }
    #endregion

    #region NavigationProperties
    public SystemAccountEntity Creator { get; set; }

    public SystemAccountEntity Updater { get; set; }

    public SystemAccountEntity Remover { get; set; }
    #endregion

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
