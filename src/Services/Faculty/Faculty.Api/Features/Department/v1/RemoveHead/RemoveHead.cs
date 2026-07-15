namespace Faculty.Api.Features.Department.v1.RemoveHead;

public static class RemoveHead
{
    public sealed record Command(Guid DepartmentId) : ICommand<Result>;

    public sealed class Handler(IDepartmentRepository departmentRepository, IUnitOfWork unitOfWork)
        : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var department = await departmentRepository.GetByIdAsync(new DepartmentId(request.DepartmentId), cancellationToken);

            if (department is null)
                return DepartmentErrors.NotFound;

            if (department.HeadProfessorId is null)
                return Result.Success();

            var removeResult = department.RemoveHead();

            if (removeResult.IsFailure)
                return removeResult.Error;

            await unitOfWork.SaveAsync(cancellationToken);

            return Result.Success();
        }
    }

    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("api/v{v:apiVersion}/departments/{departmentId:guid}/head",
                    async (Guid departmentId, ISender sender, CancellationToken cancellationToken) =>
                    {
                        var result = await sender.Send(new Command(departmentId), cancellationToken);
                        return result.ToHttpResult();
                    })
                //.RequirePermission(PermissionCodes.Faculty.DepartmentsRemoveHead)
                .Version(app, 1.0)
                .WithName("RemoveDepartmentHead")
                .WithTags("Departments");
        }
    }
}