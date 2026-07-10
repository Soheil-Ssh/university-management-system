namespace Notification.Api.Application.Contracts.Notification;

public sealed record NotificationSendContext(NotificationMessageId NotificationMessageId,
    NotificationDeliveryAttemptId DeliveryAttemptId,
    string? CorrelationId,
    string? SourceService,
    Guid? SourceEventId,
    string? SourceEventType);