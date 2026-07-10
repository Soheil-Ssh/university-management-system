namespace Notification.Api.Application.Abstractions;

public interface IPushSender
{
    NotificationProvider Provider { get; }
    Task<NotificationSendResult> SendAsync(PushNotification notification, CancellationToken cancellationToken);
}