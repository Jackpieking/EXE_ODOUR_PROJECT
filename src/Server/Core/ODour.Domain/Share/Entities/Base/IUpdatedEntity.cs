using System;

namespace ODour.Domain.Share.Entities.Base;

/// <summary>
///     Represent the base interface that all entity classes
///     that need to track the update must inherit from.
/// </summary>
public interface IUpdatedEntity
{
    DateTime UpdatedAt { get; set; }

    Guid UpdatedBy { get; set; }
}
