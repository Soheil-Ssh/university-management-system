namespace Faculty.Api.Features.Faculties.v1.Update;

public static class Update
{
    public sealed record UpdateFacultyRequest(string Name, string? Description);

    public sealed record Command(Guid Id, string Name, string? Description) : ICommand<Result>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Description).MaximumLength(500);
        }
    }

    public sealed class Handler(IFacultyRepository facultyRepository, IUnitOfWork unitOfWork)
        : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var facultyId = new FacultyId(request.Id);

            var faculty = await facultyRepository.GetByIdAsync(facultyId, cancellationToken);
            if (faculty is null)
                return FacultyErrors.NotFound;

            var name = request.Name.Trim();

            var nameChanged = !string.Equals(faculty.Name, name, StringComparison.OrdinalIgnoreCase);

            if (nameChanged && await facultyRepository.ExistsByNameAsync(name, faculty.Id, cancellationToken))
                return FacultyErrors.NameAlreadyExists;

            var updateResult = faculty.Update(name, request.Description);
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
            app.MapPut("api/v{v:apiVersion}/faculties/{id:guid}", async (Guid id, UpdateFacultyRequest request, ISender sender,
                        CancellationToken cancellationToken) =>
                    {
                        var command = new Command(id, request.Name, request.Description);
                        var result = await sender.Send(command, cancellationToken);
                        return result.ToHttpResult();
                    })
                //.RequirePermission(PermissionCodes.Faculty.FacultiesUpdate)
                .Version(app, 1.0)
                .WithName("UpdateFaculty")
                .WithTags("Faculties");
        }
    }
}