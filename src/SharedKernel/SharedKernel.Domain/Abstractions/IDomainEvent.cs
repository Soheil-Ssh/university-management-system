using MediatR;

namespace SharedKernel.Domain.Abstractions;

public interface IDomainEvent : INotification
{
    Guid EventId { get; }
    DateTime OccurredOn { get; }
    string EventType { get; }
}