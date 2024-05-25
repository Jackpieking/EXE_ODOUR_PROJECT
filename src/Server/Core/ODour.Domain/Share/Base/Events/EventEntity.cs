using System;

namespace ODour.Domain.Share.Base.Events;

public abstract class EventEntity<TStreamId>
{
    #region PrimaryKeys
    public Guid Id { get; set; }
    #endregion

    public string Type { get; set; }

    #region ForeignKeys
    public TStreamId StreamId { get; set; }
    #endregion
}
