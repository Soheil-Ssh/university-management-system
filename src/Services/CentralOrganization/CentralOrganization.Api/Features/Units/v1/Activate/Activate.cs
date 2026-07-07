namespace CentralOrganization.Api.Features.Units.v1.Activate;

public static class Activate
{
    public sealed record Command(Guid Id) : ICommand<Result>;

    public class Handler(IUnitRepository unitRepository, IUnitOfWork unitOfWork) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var unit = await unitRepository.GetById(new UnitId(request.Id), cancellationToken);

            if (unit is null)
                return UnitErrors.NotFound;

            var activateResult = unit.Activate();

            if (activateResult.IsFailure)
                return activateResult.Error;

            await unitOfWork.SaveAsync(cancellationToken);
            return Result.Success();
        }
    }

    public class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPatch("api/v{v:apiVersion}/units/{id:guid}/activate", async (ISender sender, Guid id) =>
                {
                    var command = new Command(id);
                    var result = await sender.Send(command);
                    return result.ToHttpResult();
                })
                //.RequirePermission(PermissionCodes.CentralOrganization.UnitsActivate)
                .Version(app, 1.0)
                .WithName("ActivateUnit")
                .WithTags("Units");
        }
    }
}