namespace Identity.Api.Features.Roles.v1.UpdatePermissions;

public static class UpdatePermissions
{
    public sealed record Request(List<Guid> PermissionIds);

    public sealed record Command(Guid RoleId, List<Guid> PermissionIds) : ICommand<Result>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleForEach(x => x.PermissionIds).NotEmpty();
        }
    }

    public class Handler(IRoleRepository roleRepository, IUnitOfWork unitOfWork) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            // Get current role
            var role = await roleRepository.GetByIdAsync(new RoleId(request.RoleId), cancellationToken);
            if (role is null)
                return RoleErrors.NotFound;

            // Get current role permissions
            var currentPermissions = role.RolePermissions.Select(x => x.PermissionId.Value).ToHashSet();

            // Get new permission from request
            var requestedPermissions = request.PermissionIds.ToHashSet();

            // Remove permissions that should no longer be there
            foreach (var permissionId in currentPermissions.Except(requestedPermissions))
            {
                var result = role.RemovePermission(new PermissionId(permissionId));
                if (result.IsFailure)
                    return result.Error;
            }

            // Add new permissions
            foreach (var permissionId in requestedPermissions.Except(currentPermissions))
            {
                var result = role.AddPermission(new PermissionId(permissionId));
                if (result.IsFailure)
                    return result.Error;
            }

            await unitOfWork.SaveAsync(cancellationToken);
            return Result.Success();
        }
    }

    public class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("api/v{v:apiVersion}/roles/{id:guid}/permissions", async (Guid id, [AsParameters] Request request, ISender sender) =>
            {
                var command = new Command(id, request.PermissionIds);
                var result = await sender.Send(command);
                return result.ToHttpResult();
            })
            .Version(app, 1.0)
            .WithName("UpdateRolePermissions")
            .WithTags("Roles");
        }
    }
}