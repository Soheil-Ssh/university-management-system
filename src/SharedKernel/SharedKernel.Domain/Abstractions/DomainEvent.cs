using MediatR;

namespace SharedKernel.Domain.Abstractions;

public abstract class DomainEvent : INotification
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
    public string EventType => GetType().AssemblyQualifiedName ?? GetType().FullName ?? GetType().Name;
}