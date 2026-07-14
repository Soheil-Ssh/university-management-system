namespace Faculty.Api.Features.Department.v1.UpdateContactInformation;

public static class UpdateContactInformation
{
    public sealed record UpdateDepartmentContactInformationRequest(string? Email,
        string? PhoneNumber,
        string? InternalPhoneNumber,
        string? OfficeLocation);

    public sealed record Command(Guid DepartmentId, string? Email, string? PhoneNumber, string? InternalPhoneNumber, string? OfficeLocation)
        : ICommand<Result>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.DepartmentId).NotEmpty();
            RuleFor(x => x.Email).MaximumLength(254);
            RuleFor(x => x.PhoneNumber).MaximumLength(30);
            RuleFor(x => x.InternalPhoneNumber).MaximumLength(20);
            RuleFor(x => x.OfficeLocation).MaximumLength(200);
        }
    }

    public sealed class Handler(IDepartmentRepository departmentRepository, IUnitOfWork unitOfWork)
        : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var departmentId = new DepartmentId(request.DepartmentId);

            var department = await departmentRepository.GetByIdAsync(departmentId, cancellationToken);
            if (department is null)
                return DepartmentErrors.NotFound;

            var updateResult = department.UpdateContactInformation(request.Email,
                request.PhoneNumber,
                request.InternalPhoneNumber,
                request.OfficeLocation);

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
            app.MapPut("api/v{v:apiVersion}/departments/{departmentId:guid}/contact-information",
                    async (Guid departmentId, UpdateDepartmentContactInformationRequest request, ISender sender, CancellationToken cancellationToken) =>
                    {
                        var command = new Command(departmentId, request.Email, request.PhoneNumber, request.InternalPhoneNumber, request.OfficeLocation);
                        var result = await sender.Send(command, cancellationToken);
                        return result.ToHttpResult();
                    })
                //.RequirePermission(PermissionCodes.Faculty.DepartmentsUpdate)
                .Version(app, 1.0)
                .WithName("UpdateDepartmentContactInformation")
                .WithTags("Departments");
        }
    }
}