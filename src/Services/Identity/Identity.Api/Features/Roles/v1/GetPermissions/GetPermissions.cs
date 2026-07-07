namespace Identity.Api.Features.Roles.v1.GetPermissions;

public static class GetPermissions
{
    public sealed record Response(List<PermissionDto> Permissions);
    public sealed record PermissionDto(Guid Id, string Name, string DisplayName, string Code);

    public sealed record Query(Guid Id) : IQuery<Result<Response>>;

    public class Handler(IdentityDbContext context) : IQueryHandler<Query, Result<Response>>
    {
        public async Task<Result<Response>> Handle(Query query, CancellationToken cancellationToken)
        {
            var role = await context.Roles
                .AsNoTracking()
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Id == new RoleId(query.Id), cancellationToken);
            if (role is null)
                return RoleErrors.NotFound;

            var permissionIds = role.RolePermissions.Select(rp => rp.PermissionId);

            var permissions = await context.Permissions
                .AsNoTracking()
                .Where(p => permissionIds.Contains(p.Id))
                .Select(p => new PermissionDto(p.Id.Value, p.Name, p.DisplayName, p.Code))
                .ToListAsync(cancellationToken);

            return new Response(permissions);
        }
    }

    public class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/v{v:apiVersion}/roles/{id:guid}/permissions", async (ISender sender, Guid id) =>
            {
                var query = new Query(id);
                var result = await sender.Send(query);
                return result.ToHttpResult();
            })
            .Version(app, 1.0)
            .WithName("GetRolePermissions")
            .WithTags("Roles");
        }
    }
}