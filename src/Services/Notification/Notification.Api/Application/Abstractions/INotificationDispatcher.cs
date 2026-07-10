namespace Notification.Api.Application.Abstractions;

public interface INotificationDispatcher
{
    Task<NotificationDispatchResult> DispatchAsync(NotificationMessage notification, CancellationToken cancellationToken);
}