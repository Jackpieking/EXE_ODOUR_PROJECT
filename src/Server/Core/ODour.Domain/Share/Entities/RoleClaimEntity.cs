using System;
using Microsoft.AspNetCore.Identity;
using ODour.Domain.Share.Entities.Base;

namespace ODour.Domain.Share.Entities;

public sealed class RoleClaimEntity : IdentityRoleClaim<Guid>, IEntity
{
    #region MetaData
    public static class MetaData
    {
        public const string TableName = "RoleClaims";
    }
    #endregion
}
