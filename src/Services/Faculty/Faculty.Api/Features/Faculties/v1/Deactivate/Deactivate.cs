namespace Faculty.Api.Features.Faculties.v1.Deactivate;

public static class Deactivate
{
    public sealed record Command(Guid Id) : ICommand<Result>;

    public sealed class Handler(IFacultyRepository facultyRepository, IUnitOfWork unitOfWork)
        : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var faculty = await facultyRepository.GetByIdAsync(new FacultyId(request.Id), cancellationToken);
            if (faculty is null)
                return FacultyErrors.NotFound;

            var result = faculty.Deactivate();
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
            app.MapPatch("api/v{v:apiVersion}/faculties/{id:guid}/deactivate", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    {
                        var result = await sender.Send(new Command(id), cancellationToken);
                        return result.ToHttpResult();
                    })
                //.RequirePermission(PermissionCodes.Faculty.FacultiesDeactivate)
                .Version(app, 1.0)
                .WithName("DeactivateFaculty")
                .WithTags("Faculties");
        }
    }
}