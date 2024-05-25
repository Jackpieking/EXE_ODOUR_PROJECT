using System;
using ODour.Domain.Share.Base.Entities;
using ODour.Domain.Share.Base.Events;

namespace ODour.Domain.Share.SystemAccount.Entities;

public sealed class SystemAccountTokenEventEntity : EventEntity<Guid>, IEntity, ICreatedEntity
{
    public string OldData { get; set; }

    public string NewData { get; set; }

    public DateTime CreatedAt { get; set; }

    #region ForeignKeys
    public Guid CreatedBy { get; set; }
    #endregion

    #region NavigationProperties
    public SystemAccountEntity Creator { get; set; }
    #endregion

    #region MetaData
    public static class MetaData
    {
        public const string TableName = "SystemAccountTokenEvents";

        public static class Type
        {
            public const int MaxLength = 100;

            public const int MinLength = 2;
        }

        public static class OldData
        {
            public const int MinLength = default;
        }

        public static class NewData
        {
            public const int MinLength = default;
        }
    }
    #endregion
}
