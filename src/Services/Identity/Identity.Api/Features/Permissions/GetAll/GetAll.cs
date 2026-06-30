namespace Identity.Api.Features.Permissions.GetAll;

public static class GetAll
{
    public sealed record GetAllPermissionsResponse(Guid Id, string Name, string DisplayName, string Code);

    public sealed record Query : IQuery<Result<List<GetAllPermissionsResponse>>>;

    public class Handler(IdentityDbContext context) : IQueryHandler<Query, Result<List<GetAllPermissionsResponse>>>
    {
        public async Task<Result<List<GetAllPermissionsResponse>>> Handle(Query request, CancellationToken cancellationToken) 
            => await context.Permissions
                .AsNoTracking()
                .Select(p => new GetAllPermissionsResponse(
                    p.Id.Value,
                    p.Name,
                    p.DisplayName,
                    p.Code))
                .ToListAsync(cancellationToken);
    }

    public class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/v{v:apiVersion}/permissions", async (ISender sender) =>
            {
                var result = await sender.Send(new Query());
                return result.ToHttpResult();
            })
                .Version(app, 1.0)
                .WithName("GetAllPermissions")
                .WithTags("Permissions");
        }
    }
}