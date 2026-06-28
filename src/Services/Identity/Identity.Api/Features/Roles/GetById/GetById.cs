namespace Identity.Api.Features.Roles.GetById;

public static class GetById
{
    public sealed record Query(Guid Id) : IQuery<Result<Response>>;

    public sealed record Response(Guid Id,
        string Name,
        string? Description,
        bool IsSystem,
        DateTime CreateAt,
        DateTime UpdateAt);

    public class Handler(IdentityDbContext context) : IQueryHandler<Query, Result<Response>>
    {
        public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var role = await context.Roles.AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == new RoleId(request.Id), cancellationToken);
            if (role is null)
                return RoleErrors.NotFound;

            return new Response(role.Id.Value,
                role.Name,
                role.Description,
                role.IsSystem,
                role.CreatedAt,
                role.UpdatedAt);
        }
    }

    public class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/roles/{id:guid}", async (ISender sender, Guid id) =>
            {
                var request = new Query(id);
                var result = await sender.Send(request);
                return result.ToHttpResult();
            })
            .Version(app, 1.0)
            .WithName("GetRoleById")
            .WithTags("Roles");
        }
    }
}