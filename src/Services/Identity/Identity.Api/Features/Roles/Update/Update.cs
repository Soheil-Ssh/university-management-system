namespace Identity.Api.Features.Roles.Update;

public static class Update
{
    public sealed record Request(string Name, string Description);

    public sealed record Command(Guid Id, string Name, string Description) : IRequest<Result<Guid>>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).MaximumLength(500);
        }
    }

    public class Handler(IRoleRepository roleRepository, IUnitOfWork unitOfWork)
        : IRequestHandler<Command, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            var role = await roleRepository.GetByIdAsync(new RoleId(request.Id), cancellationToken);
            if (role is null)
                return RoleErrors.NotFound;

            if (role.IsSystem)
                return RoleErrors.SystemRoleCannotBeModified;

            // Check if name is being changed to an existing name (excluding current role)
            if (!role.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase))
            {
                var exists = await roleRepository.IsExistRole(role.Id, request.Name, cancellationToken);
                if (exists)
                    return RoleErrors.AlreadyExists;
            }

            // Update name
            var nameResult = role.UpdateName(request.Name);
            if (nameResult.IsFailure)
                return nameResult.Error;

            // Update description
            var descriptionResult = role.UpdateDescription(request.Description);
            if (descriptionResult.IsFailure)
                return descriptionResult.Error;

            await unitOfWork.SaveAsync(cancellationToken);

            return role.Id.Value;
        }
    }

    public class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("api/v{v:apiVersion}/roles/{id:guid}", async (ISender sender, Guid id, Request request) =>
            {
                var command = new Command(id, request.Name, request.Description);
                var result = await sender.Send(command);
                return result.ToHttpResult();
            })
            .Version(app, 1.0)
            .WithName("UpdateRole")
            .WithTags("Roles");
        }
    }
}