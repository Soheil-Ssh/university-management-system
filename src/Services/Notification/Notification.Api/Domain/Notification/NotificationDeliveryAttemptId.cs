namespace Notification.Api.Domain.Notification;

public sealed record NotificationDeliveryAttemptId(Guid Value)
{
    public static NotificationDeliveryAttemptId New() => new(Guid.NewGuid());

}