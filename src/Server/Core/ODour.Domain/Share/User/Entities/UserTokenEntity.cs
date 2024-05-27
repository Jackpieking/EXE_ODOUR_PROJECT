using System;
using Microsoft.AspNetCore.Identity;
using ODour.Domain.Share.Base.Entities;

namespace ODour.Domain.Share.User.Entities;

public sealed class UserTokenEntity : IdentityUserToken<Guid>, IEntity
{
    public DateTime ExpiredAt { get; set; }

    #region NavigationProperties
    public UserEntity User { get; set; }
    #endregion

    #region MetaData
    public static class MetaData
    {
        public const string TableName = "UserTokens";

        public static class Value
        {
            public const int MinLength = 1;
        }
    }
    #endregion
}
