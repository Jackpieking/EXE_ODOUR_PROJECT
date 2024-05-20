using System;
using System.Collections.Generic;
using ODour.Domain.Share.Entities.Base;

namespace ODour.Domain.Share.Entities;

public sealed class UserDetailEntity : IEntity, ITemporarilyRemovedEntity
{
    #region PrimaryForeignKeys
    public Guid UserId { get; set; }
    #endregion

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string AvatarUrl { get; set; }

    public bool Gender { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime RemovedAt { get; set; }

    #region ForeignKeys
    public Guid RemovedBy { get; set; }

    public Guid AccountStatusId { get; set; }
    #endregion

    #region NavigationProperties
    public UserEntity User { get; set; }

    public AccountStatusEntity AccountStatus { get; set; }

    public SystemAccountEntity Remover { get; set; }
    #endregion

    #region NavigationCollections
    public IEnumerable<OrderEntity> OrderCreators { get; set; }

    public IEnumerable<OrderEntity> OrderUpdaters { get; set; }
    #endregion

    #region MetaData
    public static class MetaData
    {
        public const string TableName = "UserDetails";

        public static class FirstName
        {
            public const int MinLength = 2;

            public const int MaxLength = 20;
        }

        public static class LastName
        {
            public const int MinLength = 2;

            public const int MaxLength = 100;
        }

        public static class AvatarUrl
        {
            public const int MinLength = 2;
        }
    }
    #endregion
}
