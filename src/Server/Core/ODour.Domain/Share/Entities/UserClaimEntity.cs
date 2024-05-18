using System;
using Microsoft.AspNetCore.Identity;
using ODour.Domain.Share.Entities.Base;

namespace DataAccess.Entities;

public sealed class UserClaimEntity : IdentityUserClaim<Guid>, IEntity
{
    #region MetaData
    public static class MetaData
    {
        public const string TableName = "UserClaims";
    }
    #endregion
}
