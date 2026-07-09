namespace SharedKernel.Contracts.Integration.Abstractions;

public abstract record IntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;
    public string EventType => GetType().AssemblyQualifiedName ?? GetType().FullName ?? GetType().Name;
}