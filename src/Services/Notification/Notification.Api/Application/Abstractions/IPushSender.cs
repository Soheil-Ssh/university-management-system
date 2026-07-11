namespace Notification.Api.Application.Abstractions;

public interface IPushSender
{
    NotificationProvider Provider { get; }
    Task<Result<NotificationSendResult>> SendAsync(PushNotification notification, CancellationToken cancellationToken);
}