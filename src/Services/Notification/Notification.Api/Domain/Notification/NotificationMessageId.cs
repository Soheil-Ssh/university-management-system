namespace Notification.Api.Domain.Notification;

public sealed record NotificationMessageId(Guid Value)
{
    public static NotificationMessageId New() => new(Guid.NewGuid());
}