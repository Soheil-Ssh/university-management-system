namespace Faculty.Api.Features.Faculties.v1.Activate;

public static class Activate
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

            var result = faculty.Activate();
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
            app.MapPatch("api/v{v:apiVersion}/faculties/{id:guid}/activate", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    {
                        var result = await sender.Send(new Command(id), cancellationToken);
                        return result.ToHttpResult();
                    })
                //.RequirePermission(PermissionCodes.Faculty.FacultiesActivate)
                .Version(app, 1.0)
                .WithName("ActivateFaculty")
                .WithTags("Faculties");
        }
    }
}