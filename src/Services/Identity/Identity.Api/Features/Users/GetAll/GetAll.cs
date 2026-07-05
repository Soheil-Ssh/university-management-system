
namespace Identity.Api.Features.Users.GetAll;

public static class GetAll
{
    public sealed record GetAllUsersRequest(
        string? UserName,
        string? Email,
        bool? IsActive,
        DateTime? FromCreateAt,
        DateTime? ToCreateAt,
        DateTime? FromUpdateAt,
        DateTime? ToUpdateAt,
        int Page = 1,
        int PageSize = 20);

    public sealed record Response(
        Guid Id,
        string UserName,
        string Email,
        bool IsActive,
        DateTime CreateAt,
        DateTime UpdateAt);

    public sealed record Query(
        string? UserName,
        string? Email,
        bool? IsActive,
        DateTime? FromCreateAt,
        DateTime? ToCreateAt,
        DateTime? FromUpdateAt,
        DateTime? ToUpdateAt,
        int Page = 1,
        int PageSize = 20)
        : IQuery<Result<PagedResult<Response>>>;

    public class Handler(IdentityDbContext context) : IQueryHandler<Query, Result<PagedResult<Response>>>
    {
        public async Task<Result<PagedResult<Response>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.UserName))
                query = query.Where(u => u.UserName.Contains(request.UserName.Trim()));

            if (!string.IsNullOrWhiteSpace(request.Email))
                query = query.Where(u => u.Email.Value.Contains(request.Email.Trim()));

            if (request.IsActive.HasValue)
                query = query.Where(u => u.IsActive == request.IsActive.Value);

            if (request.FromCreateAt.HasValue)
                query = query.Where(u => u.CreatedAt >= request.FromCreateAt.Value);

            if (request.ToCreateAt.HasValue)
                query = query.Where(u => u.CreatedAt <= request.ToCreateAt.Value);

            if (request.FromUpdateAt.HasValue)
                query = query.Where(u => u.UpdatedAt >= request.FromUpdateAt.Value);

            if (request.ToUpdateAt.HasValue)
                query = query.Where(u => u.UpdatedAt <= request.ToUpdateAt.Value);

            return await query.AsNoTracking()
                .Select(u => new Response(
                    u.Id.Value,
                    u.UserName,
                    u.Email.Value,
                    u.IsActive,
                    u.CreatedAt,
                    u.UpdatedAt))
                .ToPagedResultAsync(request.Page, request.PageSize, cancellationToken);
        }
    }

    public class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/v{v:apiVersion}/users", async ([AsParameters] GetAllUsersRequest request, ISender sender) =>
            {
                var query = request.Adapt<Query>();
                var result = await sender.Send(query);
                return result.ToHttpResult();
            })
            .RequireAuthorization(PermissionCodes.Identity.UsersRead)
            .Version(app, 1.0)
            .WithName("GetAllUsers")
            .WithTags("Users");
        }
    }
}
