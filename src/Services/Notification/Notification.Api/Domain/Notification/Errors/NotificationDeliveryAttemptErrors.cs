namespace Notification.Api.Domain.Notification.Errors;

public class NotificationDeliveryAttemptErrors
{
    // General errors
    public static readonly Error NotificationIdEmpty = new("NotificationDeliveryAttempt.NotificationId.Empty", "Notification id cannot be empty.", ErrorType.Validation);
    public static readonly Error InvalidChannel = new("NotificationDeliveryAttempt.Channel.Invalid", "Notification channel is invalid.", ErrorType.Validation);
    public static readonly Error InvalidProvider = new("NotificationDeliveryAttempt.Provider.Invalid", "Notification provider is invalid.", ErrorType.Validation);
    public static readonly Error InvalidAttemptNumber = new("NotificationDeliveryAttempt.AttemptNumber.Invalid", "Attempt number must be greater than zero.", ErrorType.Validation);

    // Provider response errors
    public static readonly Error ProviderMessageIdTooLong = new("NotificationDeliveryAttempt.ProviderMessageId.TooLong", "Provider message id is too long.", ErrorType.Validation);

    // Status transition errors
    public static readonly Error CannotMarkFailedAttemptAsSucceeded = 
        new("NotificationDeliveryAttempt.Status.CannotMarkFailedAsSucceeded", "Failed delivery attempt cannot be marked as succeeded.", ErrorType.Validation);
    public static readonly Error CannotMarkSucceededAttemptAsFailed = 
        new("NotificationDeliveryAttempt.Status.CannotMarkSucceededAsFailed", "Succeeded delivery attempt cannot be marked as failed.", ErrorType.Validation);

    // Failure errors
    public static readonly Error ErrorCodeTooLong = new("NotificationDeliveryAttempt.ErrorCode.TooLong", "Error code is too long.", ErrorType.Validation);
    public static readonly Error ErrorMessageEmpty = 
        new("NotificationDeliveryAttempt.ErrorMessage.Empty", "Error message cannot be empty when marking delivery attempt as failed.", ErrorType.Validation);
    public static readonly Error ErrorMessageTooLong = new("NotificationDeliveryAttempt.ErrorMessage.TooLong", "Error message is too long.", ErrorType.Validation);
}