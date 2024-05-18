using System;

namespace ODour.Domain.Share.Entities.Base;

/// <summary>
///     Represent the base interface that all entity classes
///     that need to track the creation must inherit from.
/// </summary>
public interface ICreatedEntity
{
    DateTime CreatedAt { get; set; }

    Guid CreatedBy { get; set; }
}
