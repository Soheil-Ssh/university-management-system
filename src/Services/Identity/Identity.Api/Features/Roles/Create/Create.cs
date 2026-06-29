namespace Identity.Api.Features.Roles.Create;

public static class Create
{
    public sealed record Request(string Name, string DisplayName, string? Description);

    public sealed record Command(string Name, string DisplayName, string? Description) : IRequest<Result<Guid>>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Description).MaximumLength(500);
        }
    }

    public class Handler(IRoleRepository roleRepository, IUnitOfWork unitOfWork)
        : IRequestHandler<Command, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            var existRole = await roleRepository.IsExistRole(request.Name, cancellationToken);
            if (existRole)
                return RoleErrors.AlreadyExists;

            var roleResult = Role.Create(request.Name, request.DisplayName, request.Description);

            if (roleResult.IsFailure)
                return roleResult.Error;

            await roleRepository.AddAsync(roleResult.Data, cancellationToken);
            await unitOfWork.SaveAsync(cancellationToken);

            return roleResult.Data.Id.Value;
        }
    }

    public class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/v{v:apiVersion}/roles", async (ISender sender, Request request) =>
                {
                    var command = request.Adapt<Command>();
                    var result = await sender.Send(command);
                    return result.ToHttpResult();
                })
                .Version(app, 1.0)
                .WithName("CreateRole")
                .WithTags("Roles");
        }
    }
}