using Identity.Api.Infrastructure.Persistence.Contexts;

namespace Identity.Api.Features.Roles.GetAll;

public static class GetAll
{
    public sealed record GetAllRolesRequest(
        string? Name,
        string? DisplayName,
        bool? IsSystem,
        bool? IsActive,
        DateTime? FromCreateAt,
        DateTime? ToCreateAt,
        DateTime? FromUpdateAt,
        DateTime? ToUpdateAt,
        int Page = 1,
        int PageSize = 20);

    public sealed record Response(
        Guid Id,
        string Name,
        string DisplayName,
        string? Description,
        bool IsSystem,
        bool IsActive,
        DateTime CreateAt,
        DateTime UpdateAt);

    public sealed record Query(string? Name,
        string? DisplayName,
        bool? IsSystem,
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
            var query = context.Roles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Name))
                query = query.Where(r => r.Name.Contains(request.Name.Trim()));

            if (!string.IsNullOrWhiteSpace(request.DisplayName))
                query = query.Where(r => r.DisplayName.Contains(request.DisplayName.Trim()));

            if (request.IsSystem.HasValue)
                query = query.Where(r => r.IsSystem == request.IsSystem.Value);

            if (request.IsActive.HasValue)
                query = query.Where(r => r.IsActive == request.IsActive.Value);

            if (request.FromCreateAt.HasValue)
                query = query.Where(r => r.CreatedAt >= request.FromCreateAt.Value);

            if (request.ToCreateAt.HasValue)
                query = query.Where(r => r.CreatedAt <= request.ToCreateAt.Value);

            if (request.FromUpdateAt.HasValue)
                query = query.Where(r => r.UpdatedAt >= request.FromUpdateAt.Value);

            if (request.ToUpdateAt.HasValue)
                query = query.Where(r => r.UpdatedAt <= request.ToUpdateAt.Value);


            return await query.AsNoTracking()
                .Select(r => new Response(
                    r.Id.Value,
                    r.Name,
                    r.DisplayName,
                    r.Description,
                    r.IsSystem,
                    r.IsActive,
                    r.CreatedAt,
                    r.UpdatedAt))
                .ToPagedResultAsync(request.Page, request.PageSize, cancellationToken);
        }
    }

    public class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/v{v:apiVersion}/roles", async ([AsParameters] GetAllRolesRequest request, ISender sender) =>
            {
                var query = request.Adapt<Query>();
                var result = await sender.Send(query);
                return result.ToHttpResult();
            })
            .Version(app, 1.0)
            .WithName("GetAllRoles")
            .WithTags("Roles");
        }
    }
}