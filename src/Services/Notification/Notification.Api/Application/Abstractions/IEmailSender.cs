namespace Notification.Api.Application.Abstractions;

public interface IEmailSender
{
    NotificationProvider Provider { get; }
    Task<NotificationSendResult> SendAsync(EmailNotification notification, CancellationToken cancellationToken);
}