namespace SharedKernel.Domain.Abstractions;

public abstract record DomainEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string EventType => GetType().AssemblyQualifiedName ?? GetType().FullName ?? GetType().Name;
}