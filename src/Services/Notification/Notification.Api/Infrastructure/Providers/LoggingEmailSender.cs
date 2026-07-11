namespace Notification.Api.Infrastructure.Providers;

public class LoggingEmailSender(ILogger<LoggingEmailSender> logger) : IEmailSender
{
    public NotificationProvider Provider => NotificationProvider.Logging;
    public Task<Result<NotificationSendResult>> SendAsync(EmailNotification notification, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var providerMessageId = $"log-email-{Guid.NewGuid():N}";

        logger.LogInformation(
            "Notification delivery simulated. EventName={EventName} NotificationLog={NotificationLog} " +
            "NotificationMessageId={NotificationMessageId} DeliveryAttemptId={DeliveryAttemptId} " +
            "Channel={Channel} Provider={Provider} Status={Status} RecipientEmail={RecipientEmail} " +
            "CorrelationId={CorrelationId} SourceService={SourceService} SourceEventId={SourceEventId} " +
            "SourceEventType={SourceEventType} ProviderMessageId={ProviderMessageId} Subject={Subject} Body={Body}",
            "NotificationDeliverySimulated",
            true,
            notification.Context.NotificationMessageId.Value,
            notification.Context.DeliveryAttemptId.Value,
            NotificationChannel.Email,
            Provider,
            NotificationDeliveryAttemptStatus.Succeeded,
            notification.RecipientEmail,
            notification.Context.CorrelationId,
            notification.Context.SourceService,
            notification.Context.SourceEventId,
            notification.Context.SourceEventType,
            providerMessageId,
            notification.Subject,
            notification.Body);

        return Task.FromResult(Result<NotificationSendResult>.Success(new NotificationSendResult(Provider, providerMessageId)));
    }
}