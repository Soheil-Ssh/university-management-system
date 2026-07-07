using MediatR;

namespace SharedKernel.Domain.Abstractions;

public abstract class DomainEvent : INotification
{
    protected Guid EventId = Guid.NewGuid();
    public DateTime OccurredOn => DateTime.UtcNow;
    public string? EventType => GetType().AssemblyQualifiedName;
}