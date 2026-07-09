using Notification.Api.Domain.Notification.Enums;
using Notification.Api.Domain.Notification.Errors;
using SharedKernel.Domain.Result;

namespace Notification.Api.Domain.Notification;

public sealed class NotificationMessage : AggregateRoot<NotificationMessageId>
{
    private const int MaxCorrelationIdLength = 100;
    private const int MaxSourceServiceLength = 150;
    private const int MaxSourceEventTypeLength = 250;
    private const int MaxRecipientEmailLength = 320;
    private const int MaxRecipientMobileLength = 30;
    private const int MaxRecipientDeviceTokenLength = 2048;
    private const int MaxSubjectLength = 200;
    private const int MaxBodyLength = 4000;
    private const int MaxFailureReasonLength = 1000;

    public string? CorrelationId { get; private set; }
    public string? SourceService { get; private set; }
    public Guid? SourceEventId { get; private set; }
    public string? SourceEventType { get; private set; }
    public Guid? RecipientUserId { get; private set; }
    public string? RecipientEmail { get; private set; }
    public string? RecipientMobile { get; private set; }
    public string? RecipientDeviceToken { get; private set; }
    public NotificationChannel Channel { get; private set; }
    public string? Subject { get; private set; }
    public string Body { get; private set; } = null!;
    public NotificationStatus Status { get; private set; }
    public NotificationPriority Priority { get; private set; }
    public DateTime? ScheduledAt { get; private set; }
    public DateTime? SentAtUtc { get; private set; }
    public DateTime? FailedAtUtc { get; private set; }
    public string? FailureReason { get; private set; }
    public int DeliveryAttemptCount { get; private set; }


    private readonly List<NotificationDeliveryAttempt> _deliveryAttempts = [];
    public IReadOnlyCollection<NotificationDeliveryAttempt> DeliveryAttempts => _deliveryAttempts.AsReadOnly();

    private NotificationMessage() { }

    private NotificationMessage(
        NotificationMessageId id,
        string? correlationId,
        string? sourceService,
        Guid? sourceEventId,
        string? sourceEventType,
        Guid? recipientUserId,
        string? recipientEmail,
        string? recipientMobile,
        string? recipientDeviceToken,
        NotificationChannel channel,
        string? subject,
        string body,
        NotificationPriority priority,
        DateTime? scheduledAt)
        : base(id)
    {
        CorrelationId = correlationId;
        SourceService = sourceService;
        SourceEventId = sourceEventId;
        SourceEventType = sourceEventType;
        RecipientUserId = recipientUserId;
        RecipientEmail = recipientEmail;
        RecipientMobile = recipientMobile;
        RecipientDeviceToken = recipientDeviceToken;
        Channel = channel;
        Subject = subject;
        Body = body;
        Priority = priority;
        ScheduledAt = scheduledAt;

        Status = NotificationStatus.Pending;
        DeliveryAttemptCount = 0;
    }

    public static Result<NotificationMessage> CreateEmail(
        string recipientEmail,
        string subject,
        string body,
        NotificationPriority priority = NotificationPriority.Normal,
        Guid? recipientUserId = null,
        string? correlationId = null,
        string? sourceService = null,
        Guid? sourceEventId = null,
        string? sourceEventType = null,
        DateTime? scheduledAtUtc = null)
    {
        var validationResult = ValidateCommon(correlationId, sourceService, sourceEventType, body, priority);
        if (validationResult.IsFailure) return validationResult.Error;

        if (string.IsNullOrWhiteSpace(recipientEmail)) 
            return NotificationMessageErrors.EmailRecipientEmpty;

        if (recipientEmail.Length > MaxRecipientEmailLength) 
            return NotificationMessageErrors.EmailRecipientTooLong;

        if (string.IsNullOrWhiteSpace(subject)) 
            return NotificationMessageErrors.EmailSubjectEmpty;

        if (subject.Length > MaxSubjectLength) 
            return NotificationMessageErrors.EmailSubjectTooLong;

        return new NotificationMessage(
            NotificationMessageId.New(),
            TrimOrNull(correlationId),
            TrimOrNull(sourceService),
            sourceEventId,
            TrimOrNull(sourceEventType),
            recipientUserId,
            recipientEmail.Trim(),
            null,
            null,
            NotificationChannel.Email,
            subject.Trim(),
            body.Trim(),
            priority,
            scheduledAtUtc);
    }

    public static Result<NotificationMessage> CreateSms(
        string recipientMobile,
        string body,
        NotificationPriority priority = NotificationPriority.Normal,
        Guid? recipientUserId = null,
        string? correlationId = null,
        string? sourceService = null,
        Guid? sourceEventId = null,
        string? sourceEventType = null,
        DateTime? scheduledAtUtc = null)
    {
        var validationResult = ValidateCommon(correlationId, sourceService, sourceEventType, body, priority);
        if (validationResult.IsFailure) return validationResult.Error;

        if (string.IsNullOrWhiteSpace(recipientMobile)) 
            return NotificationMessageErrors.SmsRecipientEmpty;

        if (recipientMobile.Length > MaxRecipientMobileLength) 
            return NotificationMessageErrors.SmsRecipientTooLong;

        return new NotificationMessage(
            NotificationMessageId.New(),
            TrimOrNull(correlationId),
            TrimOrNull(sourceService),
            sourceEventId,
            TrimOrNull(sourceEventType),
            recipientUserId,
            null,
            recipientMobile.Trim(),
            null,
            NotificationChannel.Sms,
            null,
            body.Trim(),
            priority,
            scheduledAtUtc);
    }

    public static Result<NotificationMessage> CreatePush(
        string recipientDeviceToken,
        string? subject,
        string body,
        NotificationPriority priority = NotificationPriority.Normal,
        Guid? recipientUserId = null,
        string? correlationId = null,
        string? sourceService = null,
        Guid? sourceEventId = null,
        string? sourceEventType = null,
        DateTime? scheduledAtUtc = null)
    {
        var validationResult = ValidateCommon(correlationId, sourceService, sourceEventType, body, priority);
        if (validationResult.IsFailure) return validationResult.Error;

        if (string.IsNullOrWhiteSpace(recipientDeviceToken)) 
            return NotificationMessageErrors.PushRecipientEmpty;

        if (recipientDeviceToken.Length > MaxRecipientDeviceTokenLength) 
            return NotificationMessageErrors.PushRecipientTooLong;

        if (!string.IsNullOrWhiteSpace(subject) && subject.Length > MaxSubjectLength) 
            return NotificationMessageErrors.PushSubjectTooLong;

        return new NotificationMessage(
            NotificationMessageId.New(),
            TrimOrNull(correlationId),
            TrimOrNull(sourceService),
            sourceEventId,
            TrimOrNull(sourceEventType),
            recipientUserId,
            null,
            null,
            recipientDeviceToken.Trim(),
            NotificationChannel.Push,
            TrimOrNull(subject),
            body.Trim(),
            priority,
            scheduledAtUtc);
    }

    public bool CanBeSent()
    {
        return Status is NotificationStatus.Pending or NotificationStatus.Failed;
    }

    public bool IsScheduledForFuture()
    {
        return ScheduledAt.HasValue && ScheduledAt.Value > DateTime.UtcNow;
    }

    public Result<NotificationDeliveryAttempt> StartDeliveryAttempt(NotificationProvider provider)
    {
        if (!CanBeSent()) return NotificationMessageErrors.CannotSend;
        if (!Enum.IsDefined(provider)) return NotificationDeliveryAttemptErrors.InvalidProvider;

        var attemptNumber = DeliveryAttemptCount + 1;
        var attemptResult = NotificationDeliveryAttempt.Create(Id, Channel, provider, attemptNumber);
        if (attemptResult.IsFailure) return attemptResult.Error;

        DeliveryAttemptCount = attemptNumber;
        Status = NotificationStatus.Pending;
        FailedAtUtc = null;
        FailureReason = null;
        _deliveryAttempts.Add(attemptResult.Data);

        return attemptResult.Data;
    }

    public Result MarkAsSent(NotificationDeliveryAttempt attempt, string? providerMessageId = null)
    {
        if (attempt.NotificationMessageId != Id) 
            return NotificationMessageErrors.DeliveryAttemptDoesNotBelongToNotification;

        var attemptResult = attempt.MarkAsSucceeded(providerMessageId);
        if (attemptResult.IsFailure) return attemptResult.Error;

        Status = NotificationStatus.Sent;
        SentAtUtc = DateTime.UtcNow;
        FailedAtUtc = null;
        FailureReason = null;

        return Result.Success();
    }

    public Result MarkAsFailed(NotificationDeliveryAttempt attempt, string failureReason, string? errorCode = null)
    {
        if (attempt.NotificationMessageId != Id) 
            return NotificationMessageErrors.DeliveryAttemptDoesNotBelongToNotification;

        if (string.IsNullOrWhiteSpace(failureReason)) 
            return NotificationMessageErrors.FailureReasonEmpty;

        if (failureReason.Length > MaxFailureReasonLength) 
            return NotificationMessageErrors.FailureReasonTooLong;

        var attemptResult = attempt.MarkAsFailed(errorCode, failureReason.Trim());
        if (attemptResult.IsFailure) return attemptResult.Error;

        Status = NotificationStatus.Failed;
        FailedAtUtc = DateTime.UtcNow;
        FailureReason = failureReason.Trim();

        return Result.Success();
    }

    public Result Cancel(string reason)
    {
        if (Status == NotificationStatus.Sent) 
            return NotificationMessageErrors.CannotCancelSentNotification;

        if (string.IsNullOrWhiteSpace(reason)) 
            return NotificationMessageErrors.CancelReasonEmpty;

        if (reason.Length > MaxFailureReasonLength) 
            return NotificationMessageErrors.CancelReasonTooLong;

        Status = NotificationStatus.Cancelled;
        FailureReason = reason.Trim();
        FailedAtUtc = null;

        return Result.Success();
    }

    private static Result ValidateCommon(
        string? correlationId,
        string? sourceService,
        string? sourceEventType,
        string body,
        NotificationPriority priority)
    {
        if (!string.IsNullOrWhiteSpace(correlationId) && correlationId.Length > MaxCorrelationIdLength)
            return NotificationMessageErrors.CorrelationIdTooLong;

        if (!string.IsNullOrWhiteSpace(sourceService) && sourceService.Length > MaxSourceServiceLength)
            return NotificationMessageErrors.SourceServiceTooLong;

        if (!string.IsNullOrWhiteSpace(sourceEventType) && sourceEventType.Length > MaxSourceEventTypeLength)
            return NotificationMessageErrors.SourceEventTypeTooLong;

        if (string.IsNullOrWhiteSpace(body))
            return NotificationMessageErrors.BodyEmpty;

        if (body.Length > MaxBodyLength)
            return NotificationMessageErrors.BodyTooLong;

        if (!Enum.IsDefined(priority))
            return NotificationMessageErrors.InvalidPriority;

        return Result.Success();
    }

    private static string? TrimOrNull(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}