using MediatR;
using SharedKernel.Domain.Abstractions;

namespace SharedKernel.Abstractions.CQRS;

public interface IEventHandler<in TEvent> : INotificationHandler<TEvent>
    where TEvent : DomainEvent;