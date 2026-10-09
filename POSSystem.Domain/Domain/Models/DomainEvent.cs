using MediatR;

namespace POSSystem.Domain.Domain.Models;

/// <summary>
/// Base class for domain events following event sourcing pattern.
/// </summary>
public abstract record DomainEvent : INotification
{
    public Guid AggregateId { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}


