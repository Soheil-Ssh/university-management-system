namespace Notification.Api.Application.Abstractions;

public interface INotificationDispatcher
{
    Task<Result<NotificationDispatchResult>> DispatchAsync(NotificationMessage notification, CancellationToken cancellationToken);
}