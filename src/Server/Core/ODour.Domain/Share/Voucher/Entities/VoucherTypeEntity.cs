using System;
using System.Collections.Generic;
using ODour.Domain.Share.Base.Entities;

namespace ODour.Domain.Share.Voucher.Entities;

public sealed class VoucherTypeEntity : IEntity, ITemporarilyRemovedEntity
{
    #region PrimaryKeys
    public Guid Id { get; set; }
    #endregion

    public string Name { get; set; }

    public bool IsTemporarilyRemoved { get; set; }

    #region NavigationCollections
    public IEnumerable<VoucherEntity> Vouchers { get; set; }
    #endregion

    #region MetaData
    public static class MetaData
    {
        public const string TableName = "VoucherTypes";

        public static class Name
        {
            public const int MaxLength = 100;

            public const int MinLength = 1;
        }
    }
    #endregion
}
