using System;
using ODour.Domain.Share.Base.Entities;
using ODour.Domain.Share.Role.Entities;

namespace ODour.Domain.Share.System.Entities
{
    public sealed class SystemAccountRoleEntity : IEntity
    {
        #region PrimaryForeignKeys
        public Guid SystemAccountId { get; set; }

        public Guid RoleId { get; set; }
        #endregion

        #region NavigationProperties
        public SystemAccountEntity SystemAccount { get; set; }

        public RoleEntity Role { get; set; }
        #endregion

        #region MetaData
        public static class MetaData
        {
            public const string TableName = "SystemAccountRoles";
        }
        #endregion
    }
}
