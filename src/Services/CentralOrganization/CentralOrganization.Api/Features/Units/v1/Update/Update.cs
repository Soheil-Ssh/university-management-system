namespace CentralOrganization.Api.Features.Units.v1.Update;

public class Update
{
    public sealed record UpdateUnitRequest(string Name, string? Description);

    public sealed record Command(Guid Id, string Name, string? Description) : ICommand<Result>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Description).MaximumLength(500);
        }
    }

    public class Handler(IUnitRepository unitRepository, IUnitOfWork unitOfWork)
        : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var unit = await unitRepository.GetById(new UnitId(request.Id), cancellationToken);

            if (unit is null)
                return UnitErrors.NotFound;

            var updateResult = unit.Update(request.Name, request.Description);

            if (updateResult.IsFailure)
                return updateResult.Error;

            await unitOfWork.SaveAsync(cancellationToken);
            return Result.Success();
        }
    }

    public class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("api/v{v:apiVersion}/units/{id:guid}", async (ISender sender, Guid id, UpdateUnitRequest request) =>
                {
                    var command = new Command(id, request.Name, request.Description);
                    var result = await sender.Send(command);
                    return result.ToHttpResult();
                })
                //.RequirePermission(PermissionCodes.CentralOrganization.UnitsUpdate)
                .Version(app, 1.0)
                .WithName("UpdateUnit")
                .WithTags("Units");
        }
    }
}