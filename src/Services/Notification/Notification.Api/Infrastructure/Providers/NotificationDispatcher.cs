using Notification.Api.Application.Errors;

namespace Notification.Api.Infrastructure.Providers;

public sealed class NotificationDispatcher(IEmailSender emailSender, ISmsSender smsSender, IPushSender pushSender,
    ILogger<NotificationDispatcher> logger) : INotificationDispatcher
{
    private const int MaxErrorCodeLength = 100;
    private const int MaxErrorMessageLength = 1000;

    public async Task<Result<NotificationDispatchResult>> DispatchAsync(NotificationMessage notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        if (notification.IsScheduledForFuture())
            return NotificationDispatchErrors.ScheduledForFuture;

        var providerResult = ResolveProvider(notification.Channel);
        if (providerResult.IsFailure)
            return providerResult.Error;

        var attemptResult = notification.StartDeliveryAttempt(providerResult.Data);
        if (attemptResult.IsFailure)
            return attemptResult.Error;

        var attempt = attemptResult.Data;
        var context = CreateSendContext(notification, attempt);

        LogDispatchStarted(notification, attempt, providerResult.Data);

        var sendResult = await SendAsync(notification, context, cancellationToken);
        if (sendResult.IsFailure)
            return MarkDispatchAsFailed(notification, attempt, providerResult.Data, sendResult.Error.Code, sendResult.Error.Description);

        var markSentResult = notification.MarkAsSent(attempt, sendResult.Data.ProviderMessageId);
        if (markSentResult.IsFailure)
            return markSentResult.Error;

        LogDispatchSucceeded(notification, attempt, sendResult.Data);

        return NotificationDispatchResult.Succeeded(notification, attempt, sendResult.Data);
    }

    private async Task<Result<NotificationSendResult>> SendAsync(NotificationMessage notification, NotificationSendContext context, CancellationToken cancellationToken)
        => notification.Channel switch
        {
            NotificationChannel.Email => await SendEmailAsync(notification, context, cancellationToken),
            NotificationChannel.Sms => await SendSmsAsync(notification, context, cancellationToken),
            NotificationChannel.Push => await SendPushAsync(notification, context, cancellationToken),
            _ => NotificationMessageErrors.ChannelInvalid
        };

    private Task<Result<NotificationSendResult>> SendEmailAsync(NotificationMessage notification, NotificationSendContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(notification.RecipientEmail))
            return Task.FromResult<Result<NotificationSendResult>>(NotificationMessageErrors.EmailRecipientEmpty);

        if (string.IsNullOrWhiteSpace(notification.Subject))
            return Task.FromResult<Result<NotificationSendResult>>(NotificationMessageErrors.EmailSubjectEmpty);

        var emailNotification = new EmailNotification(context, notification.RecipientEmail, notification.Subject, notification.Body);
        return emailSender.SendAsync(emailNotification, cancellationToken);
    }

    private Task<Result<NotificationSendResult>> SendSmsAsync(NotificationMessage notification, NotificationSendContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(notification.RecipientMobile))
            return Task.FromResult<Result<NotificationSendResult>>(NotificationMessageErrors.SmsRecipientEmpty);

        var smsNotification = new SmsNotification(context, notification.RecipientMobile, notification.Body);
        return smsSender.SendAsync(smsNotification, cancellationToken);
    }

    private Task<Result<NotificationSendResult>> SendPushAsync(NotificationMessage notification, NotificationSendContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(notification.RecipientDeviceToken))
            return Task.FromResult<Result<NotificationSendResult>>(NotificationMessageErrors.PushRecipientEmpty);

        var pushNotification = new PushNotification(context, notification.RecipientDeviceToken, notification.Subject, notification.Body);
        return pushSender.SendAsync(pushNotification, cancellationToken);
    }

    private Result<NotificationDispatchResult> MarkDispatchAsFailed(NotificationMessage notification,
        NotificationDeliveryAttempt attempt,
        NotificationProvider provider,
        string errorCode,
        string errorMessage)
    {
        var normalizedErrorCode = Truncate(errorCode, MaxErrorCodeLength);
        var normalizedErrorMessage = Truncate(errorMessage, MaxErrorMessageLength);

        var markFailedResult = notification.MarkAsFailed(attempt, normalizedErrorMessage, normalizedErrorCode);

        if (markFailedResult.IsFailure)
            return markFailedResult.Error;

        logger.LogWarning(
            "Notification dispatch failed. EventName={EventName} NotificationLog={NotificationLog} " +
            "NotificationMessageId={NotificationMessageId} DeliveryAttemptId={DeliveryAttemptId} " +
            "AttemptNumber={AttemptNumber} Channel={Channel} Provider={Provider} " +
            "AttemptStatus={AttemptStatus} DispatchStatus={DispatchStatus} " +
            "ErrorCode={ErrorCode} ErrorMessage={ErrorMessage} CorrelationId={CorrelationId} " +
            "SourceService={SourceService} SourceEventId={SourceEventId} SourceEventType={SourceEventType}",
            "NotificationDispatchFailed",
            true,
            notification.Id.Value,
            attempt.Id.Value,
            attempt.AttemptNumber,
            notification.Channel,
            provider,
            attempt.Status,
            NotificationDispatchStatus.Failed,
            normalizedErrorCode,
            normalizedErrorMessage,
            notification.CorrelationId,
            notification.SourceService,
            notification.SourceEventId,
            notification.SourceEventType);

        return NotificationDispatchResult.Failed(notification,
            attempt,
            provider,
            normalizedErrorCode,
            normalizedErrorMessage);
    }

    private Result<NotificationProvider> ResolveProvider(NotificationChannel channel)
        => channel switch
        {
            NotificationChannel.Email => emailSender.Provider,
            NotificationChannel.Sms => smsSender.Provider,
            NotificationChannel.Push => pushSender.Provider,
            _ => NotificationMessageErrors.ChannelInvalid
        };

    private static NotificationSendContext CreateSendContext(NotificationMessage notification, NotificationDeliveryAttempt attempt)
        => new(notification.Id,
            attempt.Id,
            notification.CorrelationId,
            notification.SourceService,
            notification.SourceEventId,
            notification.SourceEventType);

    private void LogDispatchStarted(NotificationMessage notification, NotificationDeliveryAttempt attempt, NotificationProvider provider)
    {
        logger.LogInformation(
            "Notification dispatch started. " +
            "EventName={EventName} NotificationLog={NotificationLog} " +
            "NotificationMessageId={NotificationMessageId} " +
            "DeliveryAttemptId={DeliveryAttemptId} AttemptNumber={AttemptNumber} " +
            "Channel={Channel} Provider={Provider} Status={Status} " +
            "CorrelationId={CorrelationId} SourceService={SourceService} " +
            "SourceEventId={SourceEventId} SourceEventType={SourceEventType}",
            "NotificationDispatchStarted",
            true,
            notification.Id.Value,
            attempt.Id.Value,
            attempt.AttemptNumber,
            notification.Channel,
            provider,
            NotificationDeliveryAttemptStatus.Pending,
            notification.CorrelationId,
            notification.SourceService,
            notification.SourceEventId,
            notification.SourceEventType);
    }

    private void LogDispatchSucceeded(NotificationMessage notification, NotificationDeliveryAttempt attempt, NotificationSendResult sendResult)
    {
        logger.LogInformation(
            "Notification dispatch succeeded. " +
            "EventName={EventName} NotificationLog={NotificationLog} " +
            "NotificationMessageId={NotificationMessageId} " +
            "DeliveryAttemptId={DeliveryAttemptId} AttemptNumber={AttemptNumber} " +
            "Channel={Channel} Provider={Provider} Status={Status} " +
            "ProviderMessageId={ProviderMessageId} CorrelationId={CorrelationId} " +
            "SourceService={SourceService} SourceEventId={SourceEventId} " +
            "SourceEventType={SourceEventType}",
            "NotificationDispatchSucceeded",
            true,
            notification.Id.Value,
            attempt.Id.Value,
            attempt.AttemptNumber,
            notification.Channel,
            sendResult.Provider,
            NotificationDispatchStatus.Succeeded,
            sendResult.ProviderMessageId,
            notification.CorrelationId,
            notification.SourceService,
            notification.SourceEventId,
            notification.SourceEventType);
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return value.Length <= maxLength ? value : value[..maxLength];
    }
}