namespace Notification.Api.Features.Notifications.v1.GetById;

public static class GetById
{
    public sealed record DeliveryAttemptResponse(
        Guid Id,
        NotificationChannel Channel,
        NotificationProvider Provider,
        int AttemptNumber,
        NotificationDeliveryAttemptStatus Status,
        string? ProviderMessageId,
        string? ErrorCode,
        string? ErrorMessage,
        DateTime? CompletedAt,
        DateTime CreatedAt,
        DateTime UpdatedAt);

    public sealed record Response(
        Guid Id,
        string? CorrelationId,
        string? SourceService,
        Guid? SourceEventId,
        string? SourceEventType,
        Guid? RecipientUserId,
        string? RecipientEmail,
        string? RecipientMobile,
        string? RecipientDeviceToken,
        NotificationChannel Channel,
        string? Subject,
        string Body,
        NotificationStatus Status,
        NotificationPriority Priority,
        DateTime? ScheduledAt,
        DateTime? SentAt,
        DateTime? FailedAt,
        string? FailureReason,
        int DeliveryAttemptCount,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        IReadOnlyCollection<DeliveryAttemptResponse> DeliveryAttempts);

    public sealed record Query(Guid Id) : IQuery<Result<Response>>;

    public sealed class Handler(NotificationDbContext context)
        : IQueryHandler<Query, Result<Response>>
    {
        public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var notificationId = new NotificationMessageId(request.Id);
            var response = await context.Notifications
                .AsNoTracking()
                .Where(x => x.Id == notificationId)
                .Select(x => new Response(
                    x.Id.Value,
                    x.CorrelationId,
                    x.SourceService,
                    x.SourceEventId,
                    x.SourceEventType,
                    x.RecipientUserId,
                    x.RecipientEmail,
                    x.RecipientMobile,
                    x.RecipientDeviceToken,
                    x.Channel,
                    x.Subject,
                    x.Body,
                    x.Status,
                    x.Priority,
                    x.ScheduledAt,
                    x.SentAt,
                    x.FailedAt,
                    x.FailureReason,
                    x.DeliveryAttemptCount,
                    x.CreatedAt,
                    x.UpdatedAt,
                    x.DeliveryAttempts
                        .OrderByDescending(attempt => attempt.AttemptNumber)
                        .Select(attempt => new DeliveryAttemptResponse(
                            attempt.Id.Value,
                            attempt.Channel,
                            attempt.Provider,
                            attempt.AttemptNumber,
                            attempt.Status,
                            attempt.ProviderMessageId,
                            attempt.ErrorCode,
                            attempt.ErrorMessage,
                            attempt.CompletedAt,
                            attempt.CreatedAt,
                            attempt.UpdatedAt))
                        .ToList()))
                .FirstOrDefaultAsync(cancellationToken);

            if (response is null)
                return NotificationMessageErrors.NotFound;

            return response;
        }
    }

    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/v{v:apiVersion}/notifications/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    {
                        var query = new Query(id);
                        var result = await sender.Send(query, cancellationToken);
                        return result.ToHttpResult();
                    })
                //.RequirePermission(PermissionCodes.Notification.NotificationsRead)
                .Version(app, 1.0)
                .WithName("GetNotificationById")
                .WithTags("Notifications");
        }
    }
}