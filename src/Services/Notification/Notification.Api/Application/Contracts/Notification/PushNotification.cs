namespace Notification.Api.Application.Contracts.Notification;

public sealed record PushNotification(NotificationSendContext Context,
    string RecipientDeviceToken,
    string? Subject,
    string Body);