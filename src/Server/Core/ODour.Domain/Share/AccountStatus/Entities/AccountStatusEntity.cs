using System;
using System.Collections.Generic;
using ODour.Domain.Share.Base.Entities;
using ODour.Domain.Share.System.Entities;
using ODour.Domain.Share.User.Entities;

namespace ODour.Domain.Share.AccountStatus.Entities;

public sealed class AccountStatusEntity : IEntity, ITemporarilyRemovedEntity
{
    #region PrimaryKeys
    public Guid Id { get; set; }
    #endregion

    public string Name { get; set; }

    public bool IsTemporarilyRemoved { get; set; }

    #region NavigationCollections
    public IEnumerable<UserDetailEntity> UserDetails { get; set; }

    public IEnumerable<SystemAccountEntity> SystemAccounts { get; set; }
    #endregion

    #region MetaData
    public static class MetaData
    {
        public const string TableName = "AccountStatuses";

        public static class Name
        {
            public const int MaxLength = 50;

            public const int MinLength = 2;
        }
    }
    #endregion
}
