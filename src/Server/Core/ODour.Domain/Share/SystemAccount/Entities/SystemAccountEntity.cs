using System;
using System.Collections.Generic;
using ODour.Domain.Share.AccountStatus.Entities;
using ODour.Domain.Share.Base.Entities;
using ODour.Domain.Share.Category.Entities;
using ODour.Domain.Share.Order.Entities;
using ODour.Domain.Share.Payment.Entities;
using ODour.Domain.Share.Product.Entities;
using ODour.Domain.Share.Role.Entities;

namespace ODour.Domain.Share.SystemAccount.Entities;

public sealed class SystemAccountEntity : IEntity, ITemporarilyRemovedEntity
{
    #region PrimaryKeys
    public Guid Id { get; set; }
    #endregion

    public string UserName { get; set; }

    public string NormalizedUserName { get; set; }

    public string Email { get; set; }

    public string NormalizedEmail { get; set; }

    public string PasswordHash { get; set; }

    public int AccessFailedCount { get; set; }

    public DateTime LockoutEnd { get; set; }

    public bool IsTemporarilyRemoved { get; set; }

    #region ForeignKeys
    public Guid AccountStatusId { get; set; }
    #endregion

    #region NavigationProperties
    public AccountStatusEntity AccountStatus { get; set; }
    #endregion

    #region NavigationCollections
    public IEnumerable<SystemAccountTokenEntity> SystemAccountTokens { get; set; }
    #endregion

    #region MetaData
    public static class MetaData
    {
        public const string TableName = "SystemAccounts";

        public static class UserName
        {
            public const int MaxLength = 50;

            public const int MinLength = 2;
        }

        public static class NormalizedUserName
        {
            public const int MaxLength = 100;

            public const int MinLength = 2;
        }

        public static class Email
        {
            public const int MaxLength = 100;

            public const int MinLength = 2;
        }

        public static class NormalizedEmail
        {
            public const int MaxLength = 100;

            public const int MinLength = 2;
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
