namespace Notification.Api.Application.Contracts.Notification;

public sealed record NotificationDispatchResult(NotificationMessageId NotificationMessageId,
    NotificationDeliveryAttemptId DeliveryAttemptId,
    NotificationChannel Channel,
    NotificationProvider Provider,
    string ProviderMessageId);