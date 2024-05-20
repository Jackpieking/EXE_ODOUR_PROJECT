using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using ODour.Domain.Share.Entities.Base;

namespace ODour.Domain.Share.Entities;

public sealed class UserEntity : IdentityUser<Guid>, IEntity
{
    #region NavigationCollections
    public IEnumerable<UserRoleEntity> UserRoles { get; set; }

    public IEnumerable<UserClaimEntity> UserClaims { get; set; }

    public IEnumerable<UserLoginEntity> UserLogins { get; set; }

    public IEnumerable<UserTokenEntity> UserTokens { get; set; }
    #endregion

    #region NavigationProperties
    public UserDetailEntity UserDetail { get; set; }
    #endregion

    #region MetaData
    public static class MetaData
    {
        public const string TableName = "Users";

        public static class UserName
        {
            public const int MaxLength = 50;

            public const int MinLength = 1;
        }

        public static class NormalizedUserName
        {
            public const int MaxLength = 100;

            public const int MinLength = 1;
        }

        public static class Email
        {
            public const int MaxLength = 100;

            public const int MinLength = 1;
        }

        public static class NormalizedEmail
        {
            public const int MaxLength = 100;

            public const int MinLength = 1;
        }

        public static class PasswordHash
        {
            public const int MinLength = 1;
        }

        public static class AccessFailedCount
        {
            public const int MinValue = default;

            public const int MaxValue = int.MaxValue;
        }
    }
    #endregion
}
