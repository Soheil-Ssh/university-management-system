namespace Notification.Api.Features.Notifications.v1.GetAll;

public static class GetAll
{
    public enum NotificationSortBy
    {
        CreatedAt = 1,
        UpdatedAt = 2,
        Priority = 3,
        DeliveryAttemptCount = 4
    }

    public enum SortDirection
    {
        Asc = 1,
        Desc = 2
    }

    public sealed record GetAllNotificationsRequest(
        Guid? SourceEventId,
        Guid? RecipientUserId,
        string? CorrelationId,
        string? SourceService,
        string? SourceEventType,
        NotificationChannel? Channel,
        NotificationStatus? Status,
        NotificationPriority? Priority,
        DateTime? FromCreatedAt,
        DateTime? ToCreatedAt,
        DateTime? FromUpdatedAt,
        DateTime? ToUpdatedAt,
        NotificationSortBy SortBy = NotificationSortBy.CreatedAt,
        SortDirection SortDirection = SortDirection.Desc,
        int Page = 1,
        int PageSize = 20);

    public sealed record Response(
        Guid Id,
        string Recipient,
        NotificationChannel Channel,
        string? Subject,
        NotificationStatus Status,
        int DeliveryAttemptCount,
        DateTime CreatedAt,
        DateTime UpdatedAt);

    public sealed record Query(
        Guid? SourceEventId,
        Guid? RecipientUserId,
        string? CorrelationId,
        string? SourceService,
        string? SourceEventType,
        NotificationChannel? Channel,
        NotificationStatus? Status,
        NotificationPriority? Priority,
        DateTime? FromCreatedAt,
        DateTime? ToCreatedAt,
        DateTime? FromUpdatedAt,
        DateTime? ToUpdatedAt,
        NotificationSortBy SortBy = NotificationSortBy.CreatedAt,
        SortDirection SortDirection = SortDirection.Desc,
        int Page = 1,
        int PageSize = 20)
        : IQuery<Result<PagedResult<Response>>>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.SourceEventId).NotEmpty().When(x => x.SourceEventId.HasValue);
            RuleFor(x => x.RecipientUserId).NotEmpty().When(x => x.RecipientUserId.HasValue);
            RuleFor(x => x.CorrelationId).MaximumLength(100);
            RuleFor(x => x.SourceService).MaximumLength(150);
            RuleFor(x => x.SourceEventType).MaximumLength(250);
            RuleFor(x => x.Channel).Must(value => !value.HasValue || Enum.IsDefined(value.Value)).WithMessage("Channel is invalid.");
            RuleFor(x => x.Status).Must(value => !value.HasValue || Enum.IsDefined(value.Value)).WithMessage("Status is invalid.");
            RuleFor(x => x.Priority).Must(value => !value.HasValue || Enum.IsDefined(value.Value)).WithMessage("Priority is invalid.");
            RuleFor(x => x.SortBy).IsInEnum();
            RuleFor(x => x.SortDirection).IsInEnum();
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
            RuleFor(x => x.FromCreatedAt).Must(BeUtcOrNull).WithMessage("FromCreatedAt must be UTC.");
            RuleFor(x => x.ToCreatedAt).Must(BeUtcOrNull).WithMessage("ToCreatedAt must be UTC.");
            RuleFor(x => x.FromUpdatedAt).Must(BeUtcOrNull).WithMessage("FromUpdatedAt must be UTC.");
            RuleFor(x => x.ToUpdatedAt).Must(BeUtcOrNull).WithMessage("ToUpdatedAt must be UTC.");
            RuleFor(x => x)
                .Must(x => !x.FromCreatedAt.HasValue || !x.ToCreatedAt.HasValue || x.FromCreatedAt.Value <= x.ToCreatedAt.Value)
                .WithMessage("FromCreatedAt cannot be greater than ToCreatedAt.");
            RuleFor(x => x)
                .Must(x => !x.FromUpdatedAt.HasValue || !x.ToUpdatedAt.HasValue || x.FromUpdatedAt.Value <= x.ToUpdatedAt.Value)
                .WithMessage("FromUpdatedAt cannot be greater than ToUpdatedAt.");
        }

        private static bool BeUtcOrNull(DateTime? value)
        {
            return !value.HasValue || value.Value.Kind == DateTimeKind.Utc;
        }
    }

    public sealed class Handler(NotificationDbContext context)
        : IQueryHandler<Query, Result<PagedResult<Response>>>
    {
        public async Task<Result<PagedResult<Response>>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var query = context.Notifications.AsNoTracking();

            if (request.SourceEventId.HasValue)
                query = query.Where(x => x.SourceEventId == request.SourceEventId.Value);

            if (request.RecipientUserId.HasValue)
                query = query.Where(x => x.RecipientUserId == request.RecipientUserId.Value);

            if (!string.IsNullOrWhiteSpace(request.CorrelationId))
                query = query.Where(x => x.CorrelationId == request.CorrelationId.Trim());

            if (!string.IsNullOrWhiteSpace(request.SourceService))
                query = query.Where(x => x.SourceService == request.SourceService.Trim());

            if (!string.IsNullOrWhiteSpace(request.SourceEventType))
                query = query.Where(x => x.SourceEventType == request.SourceEventType.Trim());

            if (request.Channel.HasValue)
                query = query.Where(x => x.Channel == request.Channel.Value);

            if (request.Status.HasValue)
                query = query.Where(x => x.Status == request.Status.Value);

            if (request.Priority.HasValue)
                query = query.Where(x => x.Priority == request.Priority.Value);

            if (request.FromCreatedAt.HasValue)
                query = query.Where(x => x.CreatedAt >= request.FromCreatedAt.Value);

            if (request.ToCreatedAt.HasValue)
                query = query.Where(x => x.CreatedAt <= request.ToCreatedAt.Value);

            if (request.FromUpdatedAt.HasValue)
                query = query.Where(x => x.UpdatedAt >= request.FromUpdatedAt.Value);

            if (request.ToUpdatedAt.HasValue)
                query = query.Where(x => x.UpdatedAt <= request.ToUpdatedAt.Value);

            query = ApplySorting(query, request.SortBy, request.SortDirection);

            return await query
                .Select(x => new Response(
                    x.Id.Value,
                    x.Channel == NotificationChannel.Email ? x.RecipientEmail! : x.Channel == NotificationChannel.Sms ? x.RecipientMobile! : "Push recipient",
                    x.Channel,
                    x.Subject,
                    x.Status,
                    x.DeliveryAttemptCount,
                    x.CreatedAt,
                    x.UpdatedAt))
                .ToPagedResultAsync(request.Page, request.PageSize, cancellationToken);
        }

        private static IQueryable<NotificationMessage> ApplySorting(
            IQueryable<NotificationMessage> query,
            NotificationSortBy sortBy,
            SortDirection direction)
        {
            return (sortBy, direction) switch
            {
                (NotificationSortBy.UpdatedAt, SortDirection.Asc) =>
                    query.OrderBy(x => x.UpdatedAt).ThenBy(x => x.Id),

                (NotificationSortBy.UpdatedAt, SortDirection.Desc) =>
                    query.OrderByDescending(x => x.UpdatedAt).ThenBy(x => x.Id),

                (NotificationSortBy.Priority, SortDirection.Asc) =>
                    query.OrderBy(x => x.Priority).ThenBy(x => x.Id),

                (NotificationSortBy.Priority, SortDirection.Desc) =>
                    query.OrderByDescending(x => x.Priority).ThenBy(x => x.Id),

                (NotificationSortBy.DeliveryAttemptCount, SortDirection.Asc) =>
                    query.OrderBy(x => x.DeliveryAttemptCount).ThenBy(x => x.Id),

                (NotificationSortBy.DeliveryAttemptCount, SortDirection.Desc) =>
                    query.OrderByDescending(x => x.DeliveryAttemptCount).ThenBy(x => x.Id),

                (NotificationSortBy.CreatedAt, SortDirection.Asc) =>
                    query.OrderBy(x => x.CreatedAt).ThenBy(x => x.Id),

                _ => query.OrderByDescending(x => x.CreatedAt).ThenBy(x => x.Id)
            };
        }
    }

    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/v{v:apiVersion}/notifications",
                    async ([AsParameters] GetAllNotificationsRequest request,
                        ISender sender,
                        CancellationToken cancellationToken) =>
                    {
                        var query = request.Adapt<Query>();
                        var result = await sender.Send(query, cancellationToken);
                        return result.ToHttpResult();
                    })
                //.RequireAuthorization()
                .Version(app, 1.0)
                .WithName("GetAllNotifications")
                .WithTags("Notifications");
        }
    }
}