namespace SharedKernel.Domain.Abstractions;

public interface IAggregateRoot
{
    IReadOnlyCollection<DomainEvent> DomainEvents { get; }
    IReadOnlyList<DomainEvent> PopDomainEvents();
}