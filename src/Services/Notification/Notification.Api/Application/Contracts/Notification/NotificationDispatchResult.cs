namespace Notification.Api.Application.Contracts.Notification;

public sealed record NotificationDispatchResult(NotificationMessageId NotificationMessageId,
    NotificationDeliveryAttemptId DeliveryAttemptId,
    NotificationChannel Channel,
    NotificationProvider Provider,
    NotificationDispatchStatus Status,
    string? ProviderMessageId,
    string? ErrorCode,
    string? ErrorMessage)
{
    public bool IsSucceeded => Status == NotificationDispatchStatus.Succeeded;

    public static NotificationDispatchResult Succeeded(NotificationMessage notification, NotificationDeliveryAttempt attempt, NotificationSendResult sendResult)
        => new NotificationDispatchResult(notification.Id,
            attempt.Id,
            notification.Channel,
            sendResult.Provider,
            NotificationDispatchStatus.Succeeded,
            sendResult.ProviderMessageId,
            null,
            null);

    public static NotificationDispatchResult Failed(
        NotificationMessage notification,
        NotificationDeliveryAttempt attempt,
        NotificationProvider provider,
        string errorCode,
        string errorMessage)
        => new NotificationDispatchResult(notification.Id,
            attempt.Id,
            notification.Channel,
            provider,
            NotificationDispatchStatus.Failed,
            null,
            errorCode,
            errorMessage);
}