using System;
using ODour.Domain.Share.Base.Entities;

namespace ODour.Domain.Share.Events;

public sealed class EventSnapshotEntity : IEntity
{
    #region PrimaryForeignKeys
    public Guid EventId { get; set; }
    #endregion

    public bool IsCompleted { get; set; }

    #region NavigationProperties
    public EventEntity Event { get; set; }
    #endregion

    #region MetaData
    public static class MetaData
    {
        public const string TableName = "EventSnapshots";
    }
    #endregion
}
