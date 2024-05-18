using System;
using System.Collections.Generic;
using ODour.Domain.Share.Entities.Base;

namespace ODour.Domain.Share.Entities;

public sealed class ProductEntity
    : IEntity,
        ICreatedEntity,
        IUpdatedEntity,
        ITemporarilyRemovedEntity
{
    public Guid Id { get; set; }

    #region ForeignKeys
    public Guid CategoryId { get; set; }

    public Guid ProductStatusId { get; set; }
    #endregion

    public string Name { get; set; }

    public decimal UnitPrice { get; set; }

    public string Description { get; set; }

    public int QuantityInStock { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Guid UpdatedBy { get; set; }

    public DateTime RemovedAt { get; set; }

    public Guid RemovedBy { get; set; }
    #region NavigationProperties
    public CategoryEntity CategoryEntity { get; set; }

    public ProductStatusEntity ProductStatusEntity { get; set; }

    public SystemAccountEntity Creator { get; set; }

    public SystemAccountEntity Updater { get; set; }

    public SystemAccountEntity Remover { get; set; }
    #endregion

    #region NavigationCollections
    public IEnumerable<ProductImageEntity> ProductImageEntities { get; set; }

    public IEnumerable<OrderItemEntity> OrderItemEntities { get; set; }
    #endregion

    #region MetaData
    public static class MetaData
    {
        public const string TableName = "Products";

        public static class Name
        {
            public const int MaxLength = 200;

            public const int MinLength = 2;
        }

        public static class UnitPrice
        {
            public const int Precision = 12;

            public const int Scale = 2;
        }

        public static class Description
        {
            public const int MinLength = 2;
        }

        public static class QuantityInStock
        {
            public const int MinValue = 0;

            public const int MaxValue = int.MaxValue;
        }
    }
    #endregion
}
