using System;
using System.Collections.Generic;
using ODour.Domain.Share.Entities.Base;

namespace ODour.Domain.Share.Entities;

public sealed class SystemAccountEntity : IEntity
{
    public Guid Id { get; set; }

    public Guid AccountStatusId { get; set; }

    public string UserName { get; set; }

    public string NormalizedUserName { get; set; }

    public string Email { get; set; }

    public string NormalizedEmail { get; set; }

    public string PasswordHash { get; set; }

    public int AccessFailedCount { get; set; }

    public DateTime LockoutEnd { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    #region NavigationProperties
    public AccountStatusEntity AccountStatusEntity { get; set; }
    #endregion

    #region NavigationCollections
    #region CategoryEntity
    public IEnumerable<CategoryEntity> CategoryCreators { get; set; }

    public IEnumerable<CategoryEntity> CategoryUpdaters { get; set; }

    public IEnumerable<CategoryEntity> CategoryRemovers { get; set; }
    #endregion

    #region ProductStatusEntity
    public IEnumerable<ProductStatusEntity> ProductStatusCreators { get; set; }

    public IEnumerable<ProductStatusEntity> ProductStatusUpdaters { get; set; }

    public IEnumerable<ProductStatusEntity> ProductStatusRemovers { get; set; }
    #endregion

    #region ProductEntity
    public IEnumerable<ProductEntity> ProductCreators { get; set; }

    public IEnumerable<ProductEntity> ProductUpdaters { get; set; }

    public IEnumerable<ProductEntity> ProductRemovers { get; set; }
    #endregion

    #region OrderStatusEntity
    public IEnumerable<OrderStatusEntity> OrderStatusCreators { get; set; }

    public IEnumerable<OrderStatusEntity> OrderStatusUpdaters { get; set; }

    public IEnumerable<OrderStatusEntity> OrderStatusRemovers { get; set; }
    #endregion

    #region PaymentMethodEntity
    public IEnumerable<PaymentMethodEntity> PaymentMethodCreators { get; set; }

    public IEnumerable<PaymentMethodEntity> PaymentMethodUpdaters { get; set; }

    public IEnumerable<PaymentMethodEntity> PaymentMethodRemovers { get; set; }
    #endregion

    #region RoleDetailEntity
    public IEnumerable<RoleDetailEntity> RoleDetailCreators { get; set; }

    public IEnumerable<RoleDetailEntity> RoleDetailUpdaters { get; set; }

    public IEnumerable<RoleDetailEntity> RoleDetailRemovers { get; set; }
    #endregion

    #region UserDetailEntity
    public IEnumerable<UserDetailEntity> UserDetailRemovers { get; set; }
    #endregion

    #region SystemAccountTokenEntity
    public IEnumerable<SystemAccountTokenEntity> SystemAccountTokenEntities { get; set; }
    #endregion
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
