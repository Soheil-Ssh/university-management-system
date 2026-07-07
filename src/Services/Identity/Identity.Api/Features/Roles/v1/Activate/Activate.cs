namespace Identity.Api.Features.Roles.v1.Activate;

public static class Activate
{
    public sealed record Command(Guid Id) : ICommand<Result<Guid>>;

    public class Handler(IRoleRepository repository, IUnitOfWork unitOfWork) : ICommandHandler<Command, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            var role = await repository.GetByIdAsync(new RoleId(request.Id), cancellationToken);
            if (role is null)
                return RoleErrors.NotFound;

            var result = role.Activate();
            if (result.IsFailure)
                return result.Error;

            await unitOfWork.SaveAsync(cancellationToken);
            return role.Id.Value;
        }
    }

    public class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/v{v:apiVersion}/roles/{id:guid}/activate", async (Guid id, ISender sender) =>
            {
                var command = new Command(id);
                var result = await sender.Send(command);
                return result.ToHttpResult();
            })
            .Version(app, 1.0)
            .WithName("ActivateRole")
            .WithTags("Roles");
        }
    }
}