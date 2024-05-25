using System;
using ODour.Domain.Share.Base.Entities;

namespace ODour.Domain.Share.Role.Entities;

public sealed class RoleDetailEntity : IEntity, ITemporarilyRemovedEntity
{
    #region PrimaryForeignKeys
    public Guid RoleId { get; set; }
    #endregion

    public bool IsTemporarilyRemoved { get; set; }

    #region NavigationProperties
    public RoleEntity Role { get; set; }
    #endregion

    #region MetaData
    public static class MetaData
    {
        public const string TableName = "RoleDetails";
    }
    #endregion
}
