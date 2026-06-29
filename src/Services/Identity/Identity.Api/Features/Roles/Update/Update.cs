namespace Identity.Api.Features.Roles.Update;

public static class Update
{
    public sealed record Request(string Name, string DisplayName, string? Description);

    public sealed record Command(Guid Id, string Name, string DisplayName, string? Description) : ICommand<Result<Guid>>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Description).MaximumLength(500);
        }
    }

    public class Handler(IRoleRepository roleRepository, IUnitOfWork unitOfWork)
        : ICommandHandler<Command, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            var role = await roleRepository.GetByIdAsync(new RoleId(request.Id), cancellationToken);
            if (role is null)
                return RoleErrors.NotFound;

            // Update name if it has changed
            if (!role.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase))
            {
                var exists = await roleRepository.IsExistRole(role.Id,
                    request.Name,
                    cancellationToken);

                if (exists)
                    return RoleErrors.AlreadyExists;

                var nameResult = role.UpdateName(request.Name);

                if (nameResult.IsFailure)
                    return nameResult.Error;
            }

            // Update display name
            var displayNameResult = role.UpdateDisplayName(request.DisplayName);
            if (displayNameResult.IsFailure)
                return displayNameResult.Error;

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
                var command = new Command(id, request.Name, request.DisplayName, request.Description);
                var result = await sender.Send(command);
                return result.ToHttpResult();
            })
            .Version(app, 1.0)
            .WithName("UpdateRole")
            .WithTags("Roles");
        }
    }
}