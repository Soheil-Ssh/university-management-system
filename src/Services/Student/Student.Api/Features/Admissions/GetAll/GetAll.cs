using SharedKernel.Identity.Permissions;

namespace Student.Api.Features.Admissions.GetAll;

public class GetAll
{
    public sealed record GetAllAdmissionRequestsRequest(string? Search,
        AdmissionRequestStatus? Status,
        AdmissionRequestStep? Step,
        DateTime? FromCreatedAt,
        DateTime? ToCreatedAt,
        int Page = 1,
        int PageSize = 20);

    public sealed record Response(Guid Id,
        string TrackingCode,
        string? ApplicantFullName,
        string? ApplicantNationalCode,
        AdmissionRequestStatus Status,
        AdmissionRequestStep Step,
        DateTime? SubmittedAt,
        DateTime CreatedAt);

    public sealed record Query(string? Search,
        AdmissionRequestStatus? Status,
        AdmissionRequestStep? Step,
        DateTime? FromCreatedAt,
        DateTime? ToCreatedAt,
        int Page = 1,
        int PageSize = 20)
        : IQuery<Result<PagedResult<Response>>>;

    public class Handler(StudentDbContext context) : IQueryHandler<Query, Result<PagedResult<Response>>>
    {
        public async Task<Result<PagedResult<Response>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = context.AdmissionRequests.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();

                query = query.Where(a =>
                    a.TrackingCode.Value.Contains(search) ||
                    (
                        a.ApplicantPersonalInfo != null &&
                        (
                            a.ApplicantPersonalInfo.FirstName.Value.Contains(search) ||
                            a.ApplicantPersonalInfo.LastName.Value.Contains(search) ||
                            a.ApplicantPersonalInfo.NationalCode.Value.Contains(search)
                        )
                    ));
            }

            if (request.Status.HasValue)
                query = query.Where(a => a.Status == request.Status.Value);

            if (request.Step.HasValue)
                query = query.Where(a => a.Step == request.Step.Value);

            if (request.FromCreatedAt.HasValue)
                query = query.Where(a => a.CreatedAt >= request.FromCreatedAt.Value);

            if (request.ToCreatedAt.HasValue)
                query = query.Where(a => a.CreatedAt <= request.ToCreatedAt.Value);

            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize switch
            {
                < 1 => 20,
                > 100 => 100,
                _ => request.PageSize
            };

            return await query
                .AsNoTracking()
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new Response(
                    a.Id.Value,
                    a.TrackingCode.Value,
                    a.ApplicantPersonalInfo == null
                        ? null
                        : a.ApplicantPersonalInfo.FirstName.Value + " " + a.ApplicantPersonalInfo.LastName.Value,
                    a.ApplicantPersonalInfo == null
                        ? null
                        : a.ApplicantPersonalInfo.NationalCode.Value,
                    a.Status,
                    a.Step,
                    a.SubmittedAt,
                    a.CreatedAt))
                .ToPagedResultAsync(page, pageSize, cancellationToken);
        }
    }

    public class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/v{v:apiVersion}/admission-requests",
                    async ([AsParameters] GetAllAdmissionRequestsRequest request, ISender sender) =>
                    {
                        var query = request.Adapt<Query>();
                        var result = await sender.Send(query);
                        return result.ToHttpResult();
                    })
                .RequireAuthorization(PermissionCodes.Identity.RolesRead)
                .Version(app, 1.0)
                .WithName("GetAllAdmissionRequests")
                .WithTags("Admission Requests");
        }
    }
}