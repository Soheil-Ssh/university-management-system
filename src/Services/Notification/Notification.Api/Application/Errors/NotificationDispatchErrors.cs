namespace Notification.Api.Application.Errors;

public static class NotificationDispatchErrors
{
    public static readonly Error ScheduledForFuture = 
        new("Notification.Dispatch.ScheduledForFuture", "The notification is scheduled for a future time and cannot be dispatched yet.", ErrorType.Validation);
}