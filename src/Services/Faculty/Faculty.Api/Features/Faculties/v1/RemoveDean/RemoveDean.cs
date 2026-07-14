namespace Faculty.Api.Features.Faculties.v1.RemoveDean;

public static class RemoveDean
{
    public sealed record Command(Guid FacultyId) : ICommand<Result>;

    public sealed class Handler(IFacultyRepository facultyRepository, IUnitOfWork unitOfWork) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            var facultyId = new FacultyId(request.FacultyId);

            var faculty = await facultyRepository.GetByIdAsync(facultyId, cancellationToken);
            if (faculty is null)
                return FacultyErrors.NotFound;

            var result = faculty.RemoveDean();

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
            app.MapDelete("api/v{v:apiVersion}/faculties/{facultyId:guid}/dean", async (Guid facultyId, ISender sender, CancellationToken cancellationToken) =>
                    {
                        var command = new Command(facultyId);
                        var result = await sender.Send(command, cancellationToken);
                        return result.ToHttpResult();
                    })
                //.RequirePermission(PermissionCodes.Faculty.FacultiesRemoveDean)
                .Version(app, 1.0)
                .WithName("RemoveFacultyDean")
                .WithTags("Faculties");
        }
    }
}