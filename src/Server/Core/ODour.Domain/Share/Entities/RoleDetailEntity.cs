using System;
using ODour.Domain.Share.Entities.Base;

namespace ODour.Domain.Share.Entities;

public sealed class RoleDetailEntity
    : IEntity,
        ICreatedEntity,
        IUpdatedEntity,
        ITemporarilyRemovedEntity
{
    #region PrimaryForeignKeys
    public Guid RoleId { get; set; }
    #endregion

    public DateTime CreatedAt { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Guid UpdatedBy { get; set; }

    public DateTime RemovedAt { get; set; }

    public Guid RemovedBy { get; set; }

    #region NavigationProperties
    public RoleEntity RoleEntity { get; set; }

    public SystemAccountEntity Creator { get; set; }

    public SystemAccountEntity Updater { get; set; }

    public SystemAccountEntity Remover { get; set; }
    #endregion

    #region MetaData
    public static class MetaData
    {
        public const string TableName = "RoleDetails";
    }
    #endregion
}
