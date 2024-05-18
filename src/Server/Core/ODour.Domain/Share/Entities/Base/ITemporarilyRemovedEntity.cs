using System;

namespace ODour.Domain.Share.Entities.Base;

/// <summary>
///     Represent the base interface that all entity classes
///     that need to track the temporarily removed must inherit from.
/// </summary>
public interface ITemporarilyRemovedEntity
{
    DateTime RemovedAt { get; set; }

    Guid RemovedBy { get; set; }
}
