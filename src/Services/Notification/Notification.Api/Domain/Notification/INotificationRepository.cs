namespace Notification.Api.Domain.Notification;

public interface INotificationRepository
{
    Task AddAsync(NotificationMessage notificationMessage, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid sourceEventId, NotificationChannel channel, CancellationToken cancellationToken);
}