using System;
using Microsoft.AspNetCore.Identity;
using ODour.Domain.Share.Entities.Base;

namespace ODour.Domain.Share.Entities;

public sealed class UserRoleEntity : IdentityUserRole<Guid>, IEntity
{
    #region MetaData
    public static class MetaData
    {
        public const string TableName = "UserRoles";
    }
    #endregion
}
