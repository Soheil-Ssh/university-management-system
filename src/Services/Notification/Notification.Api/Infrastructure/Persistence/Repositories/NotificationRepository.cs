namespace Notification.Api.Infrastructure.Persistence.Repositories;

public class NotificationRepository(NotificationDbContext context) : INotificationRepository
{
    public async Task AddAsync(NotificationMessage notificationMessage, CancellationToken cancellationToken = default)
    {
        await context.Notifications.AddAsync(notificationMessage, cancellationToken);
    }

    public Task<bool> ExistsAsync(Guid sourceEventId, NotificationChannel channel, CancellationToken cancellationToken)
        => context.Notifications.AnyAsync(notification =>
                notification.SourceEventId == sourceEventId && notification.Channel == channel, cancellationToken);
}