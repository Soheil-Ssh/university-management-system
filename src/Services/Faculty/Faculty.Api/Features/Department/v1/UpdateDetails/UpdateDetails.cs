namespace Faculty.Api.Features.Department.v1.UpdateDetails;

public static class UpdateDetails
{
    public sealed record UpdateDepartmentDetailsRequest(string Name, string? ShortName, string? Description);

    public sealed record Command(Guid DepartmentId, string Name, string? ShortName, string? Description) : ICommand<Result>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.DepartmentId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.ShortName).MaximumLength(50);
            RuleFor(x => x.Description).MaximumLength(500);
        }
    }

    public sealed class Handler(IDepartmentRepository departmentRepository, IUnitOfWork unitOfWork) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var departmentId = new DepartmentId(request.DepartmentId);

            var department = await departmentRepository.GetByIdAsync(departmentId, cancellationToken);
            if (department is null)
                return DepartmentErrors.NotFound;

            var normalizedName = request.Name.Trim();
            if (department.Name != normalizedName)
            {
                var nameExists = await departmentRepository.ExistsByNameAsync(department.FacultyId, normalizedName, departmentId, cancellationToken);
                if (nameExists)
                    return DepartmentErrors.NameAlreadyExists;
            }

            var updateResult = department.UpdateDetails(normalizedName, request.ShortName, request.Description);

            if (updateResult.IsFailure)
                return updateResult.Error;

            await unitOfWork.SaveAsync(cancellationToken);
            return Result.Success();
        }
    }

    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("api/v{v:apiVersion}/departments/{departmentId:guid}/details",
                    async (Guid departmentId, UpdateDepartmentDetailsRequest request, ISender sender, CancellationToken cancellationToken) =>
                    {
                        var command = new Command(
                            departmentId,
                            request.Name,
                            request.ShortName,
                            request.Description);

                        var result = await sender.Send(
                            command,
                            cancellationToken);

                        return result.ToHttpResult();
                    })
                //.RequirePermission(PermissionCodes.Faculty.DepartmentsUpdate)
                .Version(app, 1.0)
                .WithName("UpdateDepartmentDetails")
                .WithTags("Departments");
        }
    }
}