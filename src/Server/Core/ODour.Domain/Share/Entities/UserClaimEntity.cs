using System;
using Microsoft.AspNetCore.Identity;
using ODour.Domain.Share.Entities.Base;

namespace ODour.Domain.Share.Entities;

public sealed class UserClaimEntity : IdentityUserClaim<Guid>, IEntity
{
    #region NavigationProperties
    public UserEntity User { get; set; }
    #endregion

    #region MetaData
    public static class MetaData
    {
        public const string TableName = "UserClaims";
    }
    #endregion
}
