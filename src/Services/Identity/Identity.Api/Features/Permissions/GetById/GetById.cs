namespace Identity.Api.Features.Permissions.GetById;

public static class GetById
{
    public sealed record Response(Guid Id, string Name, string DisplayName, string Code);

    public sealed record Query(Guid Id) : IQuery<Result<Response>>;

    public class Handler(IdentityDbContext context) : IQueryHandler<Query, Result<Response>>
    {
        public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var permission = await context.Permissions.AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == new PermissionId(request.Id), cancellationToken);
            if (permission is null)
                return PermissionErrors.NotFound;

            return new Response(
                permission.Id.Value,
                permission.Name,
                permission.DisplayName,
                permission.Code);
        }
    }

    public class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/v{v:apiVersion}/permissions/{id:guid}", async (ISender sender, Guid id) => 
                {
                    var request = new Query(id);
                    var result = await sender.Send(request);
                    return result.ToHttpResult();
                })
                .Version(app, 1.0)
                .WithName("GetPermissionById")
                .WithTags("Permissions");
        }
    }
}