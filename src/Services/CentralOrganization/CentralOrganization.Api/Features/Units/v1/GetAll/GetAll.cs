using SharedKernel.Api.Contracts;

namespace CentralOrganization.Api.Features.Units.v1.GetAll;

public static class GetAll
{
    public sealed record GetAllUnitsRequest(string? Code,
        string? Name,
        string? Description,
        bool? IsActive,
        DateTime? FromCreateAt,
        DateTime? ToCreateAt,
        DateTime? FromUpdateAt,
        DateTime? ToUpdateAt,
        int Page = 1,
        int PageSize = 20);

    public sealed record Response(Guid Id,
        string Code,
        string Name,
        string? Description,
        bool IsActive,
        DateTime CreateAt,
        DateTime UpdateAt);

    public sealed record Query(string? Code,
        string? Name,
        string? Description,
        bool? IsActive,
        DateTime? FromCreateAt,
        DateTime? ToCreateAt,
        DateTime? FromUpdateAt,
        DateTime? ToUpdateAt,
        int Page = 1,
        int PageSize = 20)
        : IQuery<Result<PagedResult<Response>>>;

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Code).MaximumLength(16);
            RuleFor(x => x.Name).MaximumLength(150);
            RuleFor(x => x.Description).MaximumLength(500);
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
            RuleFor(x => x.ToCreateAt).GreaterThanOrEqualTo(x => x.FromCreateAt).When(x => x.FromCreateAt.HasValue && x.ToCreateAt.HasValue);
            RuleFor(x => x.ToUpdateAt).GreaterThanOrEqualTo(x => x.FromUpdateAt).When(x => x.FromUpdateAt.HasValue && x.ToUpdateAt.HasValue);
        }
    }

    public class Handler(CentralOrganizationDbContext context) : IQueryHandler<Query, Result<PagedResult<Response>>>
    {
        public async Task<Result<PagedResult<Response>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = context.Units.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                var codeResult = UnitCode.FromString(request.Code);
                if (codeResult.IsFailure)
                    return codeResult.Error;

                query = query.Where(u => u.Code == codeResult.Data);
            }

            if (!string.IsNullOrWhiteSpace(request.Name))
                query = query.Where(u => u.Name.Contains(request.Name.Trim()));

            if (!string.IsNullOrWhiteSpace(request.Description))
                query = query.Where(u => u.Description != null &&
                                         u.Description.Contains(request.Description.Trim()));

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

            return await query
                 .AsNoTracking()
                 .OrderBy(u => u.Name)
                 .ThenBy(u => u.Id)
                 .Select(u => new Response(
                     u.Id.Value,
                     u.Code.Value,
                     u.Name,
                     u.Description,
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
            app.MapGet("api/v{v:apiVersion}/units", async ([AsParameters] GetAllUnitsRequest request, ISender sender) =>
                {
                    var query = request.Adapt<Query>();
                    var result = await sender.Send(query);
                    return result.ToHttpResult();
                })
                //.RequirePermission(PermissionCodes.CentralOrganization.UnitsRead)
                .Version(app, 1.0)
                .WithName("GetAllUnits")
                .WithTags("Units");
        }
    }
}