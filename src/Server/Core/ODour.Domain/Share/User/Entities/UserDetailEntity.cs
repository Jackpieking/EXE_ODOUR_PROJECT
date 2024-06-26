using System;
using System.Collections.Generic;
using ODour.Domain.Share.AccountStatus.Entities;
using ODour.Domain.Share.Base.Entities;
using ODour.Domain.Share.Cart.Entities;
using ODour.Domain.Share.Order.Entities;
using ODour.Domain.Share.Voucher.Entities;

namespace ODour.Domain.Share.User.Entities;

public sealed class UserDetailEntity : IEntity, ITemporarilyRemovedEntity
{
    #region PrimaryForeignKeys
    public Guid UserId { get; set; }
    #endregion

    public string AppPasswordHash { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string AvatarUrl { get; set; }

    public bool Gender { get; set; }

    public bool IsTemporarilyRemoved { get; set; }

    #region ForeignKeys
    public Guid AccountStatusId { get; set; }
    #endregion

    #region NavigationProperties
    public UserEntity User { get; set; }

    public AccountStatusEntity AccountStatus { get; set; }
    #endregion

    #region NavigationCollections
    public IEnumerable<UserVoucherEntity> UserVouchers { get; set; }

    public IEnumerable<CartItemEntity> CartItems { get; set; }

    public IEnumerable<OrderEntity> Orders { get; set; }
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

        public static class AppPasswordHash
        {
            public const int MinLength = 1;
        }
    }
    #endregion
}
