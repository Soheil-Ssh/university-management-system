
namespace Notification.Api.Application.Abstractions;

public interface ISmsSender
{
    NotificationProvider Provider { get; }
    Task<NotificationSendResult> SendAsync(SmsNotification notification, CancellationToken cancellationToken);
}