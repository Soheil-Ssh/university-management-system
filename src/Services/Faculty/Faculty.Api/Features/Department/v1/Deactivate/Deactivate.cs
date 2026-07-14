namespace Faculty.Api.Features.Department.v1.Deactivate;

public static class Deactivate
{
    public sealed record Command(Guid DepartmentId) : ICommand<Result>;

    public sealed class Handler(IDepartmentRepository departmentRepository, IUnitOfWork unitOfWork) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var departmentId = new DepartmentId(request.DepartmentId);
            var department = await departmentRepository.GetByIdAsync(departmentId, cancellationToken);
            if (department is null)
                return DepartmentErrors.NotFound;

            var result = department.Deactivate();

            if (result.IsFailure)
                return result.Error;

            await unitOfWork.SaveAsync(cancellationToken);
            return Result.Success();
        }
    }

    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPatch("api/v{v:apiVersion}/departments/{departmentId:guid}/deactivate",
                    async (Guid departmentId, ISender sender, CancellationToken cancellationToken) =>
                    {
                        var command = new Command(departmentId);
                        var result = await sender.Send(command, cancellationToken);
                        return result.ToHttpResult();
                    })
                //.RequirePermission(PermissionCodes.Faculty.DepartmentsDeactivate)
                .Version(app, 1.0)
                .WithName("DeactivateDepartment")
                .WithTags("Departments");
        }
    }
}