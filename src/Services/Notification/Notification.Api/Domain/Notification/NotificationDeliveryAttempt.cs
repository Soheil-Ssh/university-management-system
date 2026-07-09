namespace Notification.Api.Domain.Notification;

public sealed class NotificationDeliveryAttempt : Entity<NotificationDeliveryAttemptId>
{
    private const int MaxProviderMessageIdLength = 200;
    private const int MaxErrorCodeLength = 100;
    private const int MaxErrorMessageLength = 1000;

    public NotificationMessageId NotificationMessageId { get; private set; }
    public NotificationChannel Channel { get; private set; }
    public NotificationProvider Provider { get; private set; }
    public int AttemptNumber { get; private set; }
    public NotificationDeliveryAttemptStatus Status { get; private set; }
    public string? ProviderMessageId { get; private set; }
    public string? ErrorCode { get; private set; }
    public string? ErrorMessage { get; private set; }
    public DateTime? CompletedAt { get; private set; }

#pragma warning disable CS8618
    private NotificationDeliveryAttempt() { }
#pragma warning restore CS8618

    private NotificationDeliveryAttempt(
        NotificationDeliveryAttemptId id,
        NotificationMessageId notificationMessageId,
        NotificationChannel channel,
        NotificationProvider provider,
        int attemptNumber)
        : base(id)
    {
        NotificationMessageId = notificationMessageId;
        Channel = channel;
        Provider = provider;
        AttemptNumber = attemptNumber;
        Status = NotificationDeliveryAttemptStatus.Pending;
    }

    public static Result<NotificationDeliveryAttempt> Create(
        NotificationMessageId notificationId,
        NotificationChannel channel,
        NotificationProvider provider,
        int attemptNumber)
    {
        if (notificationId.Value == Guid.Empty)
            return NotificationDeliveryAttemptErrors.NotificationIdEmpty;

        if (!Enum.IsDefined(channel))
            return NotificationDeliveryAttemptErrors.InvalidChannel;

        if (!Enum.IsDefined(provider))
            return NotificationDeliveryAttemptErrors.InvalidProvider;

        if (attemptNumber <= 0)
            return NotificationDeliveryAttemptErrors.InvalidAttemptNumber;

        return new NotificationDeliveryAttempt(
            NotificationDeliveryAttemptId.New(),
            notificationId,
            channel,
            provider,
            attemptNumber);
    }

    public Result MarkAsSucceeded(string? providerMessageId = null)
    {
        if (Status == NotificationDeliveryAttemptStatus.Succeeded) return Result.Success();

        if (Status == NotificationDeliveryAttemptStatus.Failed)
            return NotificationDeliveryAttemptErrors.CannotMarkFailedAttemptAsSucceeded;

        if (!string.IsNullOrWhiteSpace(providerMessageId) && providerMessageId.Length > MaxProviderMessageIdLength)
            return NotificationDeliveryAttemptErrors.ProviderMessageIdTooLong;

        Status = NotificationDeliveryAttemptStatus.Succeeded;
        ProviderMessageId = TrimOrNull(providerMessageId);
        ErrorCode = null;
        ErrorMessage = null;
        CompletedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public Result MarkAsFailed(string? errorCode, string errorMessage)
    {
        if (Status == NotificationDeliveryAttemptStatus.Succeeded)
            return NotificationDeliveryAttemptErrors.CannotMarkSucceededAttemptAsFailed;

        if (Status == NotificationDeliveryAttemptStatus.Failed) return Result.Success();

        if (!string.IsNullOrWhiteSpace(errorCode) && errorCode.Length > MaxErrorCodeLength)
            return NotificationDeliveryAttemptErrors.ErrorCodeTooLong;

        if (string.IsNullOrWhiteSpace(errorMessage))
            return NotificationDeliveryAttemptErrors.ErrorMessageEmpty;

        if (errorMessage.Length > MaxErrorMessageLength)
            return NotificationDeliveryAttemptErrors.ErrorMessageTooLong;

        Status = NotificationDeliveryAttemptStatus.Failed;
        ErrorCode = TrimOrNull(errorCode);
        ErrorMessage = errorMessage.Trim();
        CompletedAt = DateTime.UtcNow;

        return Result.Success();
    }

    private static string? TrimOrNull(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}