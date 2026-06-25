namespace SharedKernel.Domain.Abstractions;

public interface IAggregateRoot
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    IReadOnlyList<IDomainEvent> PopDomainEvents();
}