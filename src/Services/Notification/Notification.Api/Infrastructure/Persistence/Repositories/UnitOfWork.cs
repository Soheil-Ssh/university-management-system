namespace Notification.Api.Infrastructure.Persistence.Repositories;

public class UnitOfWork(NotificationDbContext context) : IUnitOfWork
{
    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}