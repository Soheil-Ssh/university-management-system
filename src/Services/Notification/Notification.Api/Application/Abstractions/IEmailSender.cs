namespace Notification.Api.Application.Abstractions;

public interface IEmailSender
{
    NotificationProvider Provider { get; }
    Task<Result<NotificationSendResult>> SendAsync(EmailNotification notification, CancellationToken cancellationToken);
}