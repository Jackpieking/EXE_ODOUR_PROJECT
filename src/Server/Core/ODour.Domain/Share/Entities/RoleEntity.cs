using System;
using Microsoft.AspNetCore.Identity;
using ODour.Domain.Share.Entities.Base;

namespace ODour.Domain.Share.Entities;

public sealed class RoleEntity : IdentityRole<Guid>, IEntity
{
    #region NavigationProperties
    public RoleDetailEntity RoleDetailEntity { get; set; }
    #endregion

    #region MetaData
    public static class MetaData
    {
        public const string TableName = "Roles";
    }
    #endregion
}
