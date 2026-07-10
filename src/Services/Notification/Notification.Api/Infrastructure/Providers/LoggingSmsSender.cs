namespace Notification.Api.Infrastructure.Providers;

public class LoggingSmsSender(ILogger<LoggingSmsSender> logger) : ISmsSender
{
    public NotificationProvider Provider => NotificationProvider.Logging;

    public Task<NotificationSendResult> SendAsync(SmsNotification notification, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var providerMessageId = $"log-sms-{Guid.NewGuid():N}";

        logger.LogInformation(
            "Notification delivery simulated. EventName={EventName} NotificationLog={NotificationLog} " +
            "NotificationMessageId={NotificationMessageId} DeliveryAttemptId={DeliveryAttemptId} " +
            "Channel={Channel} Provider={Provider} Status={Status} RecipientMobile={RecipientMobile} " +
            "CorrelationId={CorrelationId} SourceService={SourceService} SourceEventId={SourceEventId} " +
            "SourceEventType={SourceEventType} ProviderMessageId={ProviderMessageId} Body={Body}",
            "NotificationDeliverySimulated",
            true,
            notification.Context.NotificationMessageId.Value,
            notification.Context.DeliveryAttemptId.Value,
            NotificationChannel.Sms,
            Provider,
            NotificationDeliveryAttemptStatus.Succeeded,
            notification.RecipientMobile,
            notification.Context.CorrelationId,
            notification.Context.SourceService,
            notification.Context.SourceEventId,
            notification.Context.SourceEventType,
            providerMessageId,
            notification.Body);

        var result =
            new NotificationSendResult(Provider, providerMessageId);

        return Task.FromResult(result);
    }
}