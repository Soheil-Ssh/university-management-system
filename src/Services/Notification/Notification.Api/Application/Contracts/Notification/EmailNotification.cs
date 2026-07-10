namespace Notification.Api.Application.Contracts.Notification;

public sealed record EmailNotification(NotificationSendContext Context,
    string RecipientEmail,
    string Subject,
    string Body);