using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using ODour.Domain.Share.Entities.Base;

namespace ODour.Domain.Share.Entities;

public sealed class RoleEntity : IdentityRole<Guid>, IEntity
{
    #region NavigationProperties
    public RoleDetailEntity RoleDetail { get; set; }
    #endregion

    #region NavigationCollections
    public IEnumerable<UserRoleEntity> UserRoles { get; set; }

    public IEnumerable<RoleClaimEntity> RoleClaims { get; set; }
    #endregion

    #region MetaData
    public static class MetaData
    {
        public const string TableName = "Roles";
    }
    #endregion
}
