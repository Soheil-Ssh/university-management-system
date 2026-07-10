namespace Notification.Api.Application.Contracts.Notification;

public sealed record NotificationSendResult(NotificationProvider Provider, string ProviderMessageId);