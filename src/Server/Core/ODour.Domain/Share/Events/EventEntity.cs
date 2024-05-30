using System;
using ODour.Domain.Share.Base.Entities;

namespace ODour.Domain.Share.Events
{
    public sealed class EventEntity : IEntity, ICreatedEntity
    {
        #region PrimaryKeys
        public Guid Id { get; set; }
        #endregion

        public string Type { get; set; }

        #region ForeignKeys
        public string StreamId { get; set; }
        #endregion

        public string Data { get; set; }

        public DateTime CreatedAt { get; set; }

        #region ForeignKeys
        public Guid CreatedBy { get; set; }
        #endregion

        #region NavigationProperties
        public EventSnapshotEntity EventSnapshot { get; set; }
        #endregion

        #region MetaData
        public static class MetaData
        {
            public const string TableName = "Events";

            public static class Type
            {
                public const int MaxLength = 200;

                public const int MinLength = 2;
            }

            public static class StreamId
            {
                public const int MinLength = 1;
            }

            public static class Data
            {
                public const int MinLength = default;
            }
        }
        #endregion
    }
}
