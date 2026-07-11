
namespace Notification.Api.Application.Abstractions;

public interface ISmsSender
{
    NotificationProvider Provider { get; }
    Task<Result<NotificationSendResult>> SendAsync(SmsNotification notification, CancellationToken cancellationToken);
}