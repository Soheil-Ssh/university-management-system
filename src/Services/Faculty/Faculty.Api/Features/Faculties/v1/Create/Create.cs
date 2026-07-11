namespace Faculty.Api.Features.Faculties.v1.Create;

public static class Create
{
    public sealed record CreateFacultyRequest(string Name, string? Description);

    public sealed record Command(string Name, string? Description) : ICommand<Result<Guid>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Description).MaximumLength(500);
        }
    }

    public sealed class Handler(IFacultyRepository facultyRepository, IUnitOfWork unitOfWork)
        : ICommandHandler<Command, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            var name = request.Name.Trim();

            if (await facultyRepository.ExistsByNameAsync(name, cancellationToken: cancellationToken))
                return FacultyErrors.NameAlreadyExists;

            var nextCodeNumber = await facultyRepository.GetNextFacultyCodeAsync(cancellationToken);

            var codeResult = FacultyCode.Create(nextCodeNumber);
            if (codeResult.IsFailure)
                return codeResult.Error;

            var facultyResult = Domain.Faculty.Faculty.Create(codeResult.Data, name, request.Description);
            if (facultyResult.IsFailure)
                return facultyResult.Error;

            await facultyRepository.AddAsync(facultyResult.Data, cancellationToken);
            await unitOfWork.SaveAsync(cancellationToken);

            return facultyResult.Data.Id.Value;
        }
    }

    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/v{v:apiVersion}/faculties", async (ISender sender, CreateFacultyRequest request, CancellationToken cancellationToken) =>
                    {
                        var command = request.Adapt<Command>();
                        var result = await sender.Send(command, cancellationToken);
                        return result.ToHttpResult();
                    })
                //.RequirePermission(PermissionCodes.Faculties.Create)
                .Version(app, 1.0)
                .WithName("CreateFaculty")
                .WithTags("Faculties");
        }
    }
}