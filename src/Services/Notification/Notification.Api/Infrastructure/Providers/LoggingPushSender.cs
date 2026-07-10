namespace Notification.Api.Infrastructure.Providers;

public class LoggingPushSender(ILogger<LoggingPushSender> logger) : IPushSender
{
    public NotificationProvider Provider => NotificationProvider.Logging;

    public Task<NotificationSendResult> SendAsync(
        PushNotification notification,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var providerMessageId = $"log-push-{Guid.NewGuid():N}";

        logger.LogInformation(
            "Notification delivery simulated. EventName={EventName} NotificationLog={NotificationLog} " +
            "NotificationMessageId={NotificationMessageId} DeliveryAttemptId={DeliveryAttemptId} " +
            "Channel={Channel} Provider={Provider} Status={Status} RecipientDeviceToken={RecipientDeviceToken} " +
            "CorrelationId={CorrelationId} SourceService={SourceService} SourceEventId={SourceEventId} " +
            "SourceEventType={SourceEventType} ProviderMessageId={ProviderMessageId} Subject={Subject} Body={Body}",
            "NotificationDeliverySimulated",
            true,
            notification.Context.NotificationMessageId.Value,
            notification.Context.DeliveryAttemptId.Value,
            NotificationChannel.Push,
            Provider,
            NotificationDeliveryAttemptStatus.Succeeded,
            notification.RecipientDeviceToken,
            notification.Context.CorrelationId,
            notification.Context.SourceService,
            notification.Context.SourceEventId,
            notification.Context.SourceEventType,
            providerMessageId,
            notification.Subject,
            notification.Body);

        var result = new NotificationSendResult(Provider, providerMessageId);
        return Task.FromResult(result);
    }
}