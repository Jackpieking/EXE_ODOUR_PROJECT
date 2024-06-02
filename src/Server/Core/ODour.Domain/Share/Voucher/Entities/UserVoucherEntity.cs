using System;
using ODour.Domain.Share.Base.Entities;
using ODour.Domain.Share.User.Entities;

namespace ODour.Domain.Share.Voucher.Entities;

public sealed class UserVoucherEntity : IEntity
{
    #region PrimaryForeignKeys
    public Guid UserId { get; set; }

    public string VoucherCode { get; set; }
    #endregion

    #region NavigationProperties
    public UserDetailEntity UserDetail { get; set; }

    public VoucherEntity Voucher { get; set; }
    #endregion

    #region MetaData
    public static class MetaData
    {
        public const string TableName = "UserVouchers";

        public static class VoucherCode
        {
            public const int MaxLength = 30;

            public const int MinLength = 1;
        }
    }
    #endregion
}
