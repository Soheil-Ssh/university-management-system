namespace SharedKernel.Domain.Abstractions;

public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot where TKey : notnull
{
    private readonly List<DomainEvent> _domainEvents = [];

    protected AggregateRoot(TKey id)
        : base(id) { }

    protected AggregateRoot() { }

    public IReadOnlyCollection<DomainEvent> DomainEvents =>
        _domainEvents.AsReadOnly();

    public IReadOnlyList<DomainEvent> PopDomainEvents()
    {
        var events = _domainEvents.ToList();
        _domainEvents.Clear();
        return events;
    }

    protected void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}