namespace CentralOrganization.Api.Features.Units.v1.GetById;

public static class GetById
{
    public sealed record Response(
        Guid Id,
        string Code,
        string Name,
        string? Description,
        bool IsActive,
        DateTime CreatedAt,
        DateTime UpdatedAt);

    public sealed record Query(Guid Id) : IQuery<Result<Response>>;

    public class Handler(CentralOrganizationDbContext context) : IQueryHandler<Query, Result<Response>>
    {
        public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var unit = await context.Units
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == new UnitId(request.Id), cancellationToken);

            if (unit is null)
                return UnitErrors.NotFound;

            return new Response(
                unit.Id.Value,
                unit.Code.Value,
                unit.Name,
                unit.Description,
                unit.IsActive,
                unit.CreatedAt,
                unit.UpdatedAt);
        }
    }

    public class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/v{v:apiVersion}/units/{id:guid}", async (ISender sender, Guid id) =>
                {
                    var query = new Query(id);
                    var result = await sender.Send(query);
                    return result.ToHttpResult();
                })
                //.RequirePermission(PermissionCodes.CentralOrganization.UnitsRead)
                .Version(app, 1.0)
                .WithName("GetUnitById")
                .WithTags("Units");
        }
    }
}