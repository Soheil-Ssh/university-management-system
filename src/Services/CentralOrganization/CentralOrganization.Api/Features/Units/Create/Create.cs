using Unit = CentralOrganization.Api.Domain.Unit.Unit;

namespace CentralOrganization.Api.Features.Units.Create;

public class Create
{
    public sealed record CreateUnitRequest(string Name, string? Description);

    public sealed record Command(string Name, string? Description) : ICommand<Result<Guid>>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Description).MaximumLength(500);
        }
    }

    public class Handler(IUnitRepository unitRepository, IUnitOfWork unitOfWork)
        : ICommandHandler<Command, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            var nextCode = await unitRepository.GetNextUnitCodeAsync(cancellationToken);

            var codeResult = UnitCode.Create(nextCode).WithPath(nameof(Unit.Code));
            if (codeResult.IsFailure)
                return codeResult.Error;

            var existsCode = await unitRepository.IsExistCodeAsync(codeResult.Data, cancellationToken);
            if (existsCode)
                return UnitErrors.CodeAlreadyExists;

            var unitResult = Unit.Create(
                codeResult.Data,
                request.Name,
                request.Description);

            if (unitResult.IsFailure)
                return unitResult.Error;

            await unitRepository.AddAsync(unitResult.Data, cancellationToken);
            await unitOfWork.SaveAsync(cancellationToken);

            return unitResult.Data.Id.Value;
        }
    }

    public class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/v{v:apiVersion}/units", async (ISender sender, CreateUnitRequest request) =>
                {
                    var command = request.Adapt<Command>();
                    var result = await sender.Send(command);
                    return result.ToHttpResult();
                })
                //.RequirePermission(PermissionCodes.CentralOrganization.UnitsCreate)
                .Version(app, 1.0)
                .WithName("CreateUnit")
                .WithTags("Units");
        }
    }
}