using System;
using ODour.Domain.Share.Base.Entities;

namespace ODour.Domain.Share.EventSnapshot.Entities;

public sealed class EventSnapshotEntity : IEntity
{
    #region PrimaryKeys
    public Guid EventId { get; set; }

    public string StreamId { get; set; }

    public bool IsCompleted { get; set; }
    #endregion

    #region MetaData
    public static class MetaData
    {
        public const string TableName = "EventSnapshots";

        public static class StreamId
        {
            public const int MinLength = 1;
        }
    }
    #endregion
}
