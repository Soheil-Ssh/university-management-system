namespace Notification.Api.Domain.Notification.Errors;

public class NotificationMessageErrors
{
    // General errors
    public static readonly Error NotFound = new("Notification.NotFound", "Notification was not found.", ErrorType.Validation);

    // Body errors
    public static readonly Error BodyEmpty = new("Notification.Body.Empty", "Notification body cannot be empty.", ErrorType.Validation);
    public static readonly Error BodyTooLong = new("Notification.Body.TooLong", "Notification body is too long.", ErrorType.Validation);

    // Priority errors
    public static readonly Error InvalidPriority = new("Notification.Priority.Invalid", "Notification priority is invalid.", ErrorType.Validation);

    // Correlation errors
    public static readonly Error CorrelationIdTooLong = new("Notification.CorrelationId.TooLong", "Correlation id is too long.", ErrorType.Validation);

    // Source errors
    public static readonly Error SourceServiceTooLong = new("Notification.SourceService.TooLong", "Source service is too long.", ErrorType.Validation);
    public static readonly Error SourceEventTypeTooLong = new("Notification.SourceEventType.TooLong", "Source event type is too long.", ErrorType.Validation);

    // Email notification errors
    public static readonly Error EmailRecipientEmpty = 
        new("Notification.Email.RecipientEmpty", "Recipient email cannot be empty for email notification.", ErrorType.Validation);
    public static readonly Error EmailRecipientTooLong = new("Notification.Email.RecipientTooLong", "Recipient email is too long.", ErrorType.Validation);
    public static readonly Error EmailSubjectEmpty = new("Notification.Email.SubjectEmpty", "Email subject cannot be empty.", ErrorType.Validation);
    public static readonly Error EmailSubjectTooLong = new("Notification.Email.SubjectTooLong", "Email subject is too long.", ErrorType.Validation);

    // SMS notification errors
    public static readonly Error SmsRecipientEmpty = 
        new("Notification.Sms.RecipientEmpty", "Recipient mobile number cannot be empty for SMS notification.", ErrorType.Validation);
    public static readonly Error SmsRecipientTooLong = new("Notification.Sms.RecipientTooLong", "Recipient mobile number is too long.", ErrorType.Validation);

    // Push notification errors
    public static readonly Error PushRecipientEmpty = 
        new("Notification.Push.RecipientEmpty", "Recipient device token cannot be empty for push notification.", ErrorType.Validation);
    public static readonly Error PushRecipientTooLong = new("Notification.Push.RecipientTooLong", "Recipient device token is too long.", ErrorType.Validation);
    public static readonly Error PushSubjectTooLong = new("Notification.Push.SubjectTooLong", "Push notification subject is too long.", ErrorType.Validation);

    // Delivery errors
    public static readonly Error CannotSend = new("Notification.Delivery.CannotSend", "Only pending or failed notifications can be sent.", ErrorType.Validation);
    public static readonly Error DeliveryAttemptEmpty = new("Notification.Delivery.AttemptEmpty", "Delivery attempt cannot be empty.", ErrorType.Validation);
    public static readonly Error DeliveryAttemptDoesNotBelongToNotification = 
        new("Notification.Delivery.AttemptDoesNotBelongToNotification", "Delivery attempt does not belong to this notification.", ErrorType.Validation);
    public static readonly Error FailureReasonEmpty = new("Notification.Delivery.FailureReasonEmpty", "Failure reason cannot be empty.", ErrorType.Validation);
    public static readonly Error FailureReasonTooLong = new("Notification.Delivery.FailureReasonTooLong", "Failure reason is too long.", ErrorType.Validation);

    // Cancellation errors
    public static readonly Error CancelReasonEmpty = new("Notification.Cancel.ReasonEmpty", "Cancel reason cannot be empty.", ErrorType.Validation);
    public static readonly Error CancelReasonTooLong = new("Notification.Cancel.ReasonTooLong", "Cancel reason is too long.", ErrorType.Validation);
    public static readonly Error CannotCancelSentNotification = 
        new("Notification.Cancel.CannotCancelSentNotification", "Sent notification cannot be cancelled.", ErrorType.Validation);

    // Channel errors
    public static readonly Error ChannelInvalid = new("Notification.Channel.Invalid", "Channel invalid.", ErrorType.Validation);
}