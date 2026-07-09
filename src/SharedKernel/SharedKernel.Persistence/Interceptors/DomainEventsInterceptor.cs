using MediatR;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SharedKernel.Domain.Abstractions;

namespace SharedKernel.Persistence.Interceptors;

public sealed class DomainEventsInterceptor(IMediator mediator) : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
            return await base.SavingChangesAsync(eventData, result, cancellationToken);

        var domainEvents = eventData.Context.ChangeTracker
            .Entries<IAggregateRoot>()
            .Select(entry => entry.Entity)
            .SelectMany(aggregate => aggregate.PopDomainEvents())
            .ToList();

        if (domainEvents.Count == 0)
            return await base.SavingChangesAsync(eventData, result, cancellationToken);

        foreach (var domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent, cancellationToken);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}