using System;
using Microsoft.AspNetCore.Identity;
using ODour.Domain.Share.Base.Entities;

namespace ODour.Domain.Share.Role.Entities;

public sealed class RoleClaimEntity : IdentityRoleClaim<Guid>, IEntity
{
    #region NavigationProperties
    public RoleEntity Role { get; set; }
    #endregion

    #region MetaData
    public static class MetaData
    {
        public const string TableName = "RoleClaims";
    }
    #endregion
}
