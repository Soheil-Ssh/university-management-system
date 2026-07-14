using Faculty.Api.Domain.Department.Errors;

namespace Faculty.Api.Features.Department.v1.Create;

public static class Create
{
    public sealed record CreateDepartmentRequest(Guid FacultyId,
        string Name,
        string? ShortName,
        string? Description,
        string? Email,
        string? PhoneNumber,
        string? InternalPhoneNumber,
        string? OfficeLocation);

    public sealed record Response(Guid Id, string Code);

    public sealed record Command(Guid FacultyId,
        string Name,
        string? ShortName,
        string? Description,
        string? Email,
        string? PhoneNumber,
        string? InternalPhoneNumber,
        string? OfficeLocation) : ICommand<Result<Response>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.FacultyId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.ShortName).MaximumLength(50);
            RuleFor(x => x.Description).MaximumLength(500);
            RuleFor(x => x.Email).MaximumLength(254);
            RuleFor(x => x.PhoneNumber).MaximumLength(30);
            RuleFor(x => x.InternalPhoneNumber).MaximumLength(20);
            RuleFor(x => x.OfficeLocation).MaximumLength(200);
        }
    }

    public sealed class Handler(IFacultyRepository facultyRepository, IDepartmentRepository departmentRepository, IUnitOfWork unitOfWork)
       : ICommandHandler<Command, Result<Response>>
    {
        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var facultyId = new FacultyId(request.FacultyId);

            var faculty = await facultyRepository.GetByIdAsync(facultyId, cancellationToken);
            if (faculty is null)
                return FacultyErrors.NotFound;

            if (!faculty.IsActive)
                return FacultyErrors.CannotAddDepartmentToInactiveFaculty;

            var normalizedName = request.Name.Trim();

            var nameExists = await departmentRepository.ExistsByNameAsync(facultyId, normalizedName, cancellationToken);
            if (nameExists)
                return DepartmentErrors.NameAlreadyExists;

            var nextCodeNumber = await departmentRepository.GetNextDepartmentCodeAsync(cancellationToken);

            var codeResult = DepartmentCode.Create(nextCodeNumber);
            if (codeResult.IsFailure)
                return codeResult.Error;

            var departmentResult = Domain.Department.Department.Create(
                facultyId,
                codeResult.Data,
                normalizedName,
                request.ShortName,
                request.Description,
                request.Email,
                request.PhoneNumber,
                request.InternalPhoneNumber,
                request.OfficeLocation);

            if (departmentResult.IsFailure)
                return departmentResult.Error;

            var department = departmentResult.Data;

            await departmentRepository.AddAsync(department, cancellationToken);
            await unitOfWork.SaveAsync(cancellationToken);

            return new Response(department.Id.Value, department.Code.Value);
        }
    }

    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/v{v:apiVersion}/departments",
                    async (CreateDepartmentRequest request, ISender sender, CancellationToken cancellationToken) =>
                    {
                        var command = request.Adapt<Command>();
                        var result = await sender.Send(command, cancellationToken);
                        return result.ToHttpResult();
                    })
                //.RequirePermission(PermissionCodes.Faculty.DepartmentsCreate)
                .Version(app, 1.0)
                .WithName("AddDepartment")
                .WithTags("Departments");
        }
    }
}