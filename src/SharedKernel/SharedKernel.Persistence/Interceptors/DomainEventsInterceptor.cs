using MediatR;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SharedKernel.Domain.Abstractions;

namespace SharedKernel.Persistence.Interceptors;

public sealed class DomainEventsInterceptor(IMediator mediator) : SaveChangesInterceptor
{
    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
            return await base.SavedChangesAsync(eventData, result, cancellationToken);

        var domainEvents = eventData.Context.ChangeTracker
            .Entries<IAggregateRoot>()
            .Select(entry => entry.Entity)
            .SelectMany(aggregate => aggregate.PopDomainEvents())
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent, cancellationToken);
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }
}