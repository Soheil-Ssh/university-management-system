namespace Notification.Api.Application.Contracts.Notification;

public sealed record SmsNotification(NotificationSendContext Context, string RecipientMobile, string Body);