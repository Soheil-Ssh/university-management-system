namespace Faculty.Api.Features.Faculties.v1.GetById;

public static class GetById
{
    public sealed record Query(Guid Id) : IQuery<Result<Response>>;

    public sealed record Response(Guid Id,
        string Code,
        string Name,
        string? Description,
        bool IsActive,
        DateTime CreatedAt,
        DateTime UpdatedAt);

    public sealed class Handler(FacultyDbContext dbContext)
        : IQueryHandler<Query, Result<Response>>
    {
        public async Task<Result<Response>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var facultyId = new FacultyId(request.Id);
            var faculty = await dbContext.Faculties
                .AsNoTracking()
                .Where(f => f.Id == facultyId)
                .Select(f => new Response(
                    f.Id.Value,
                    f.Code,
                    f.Name,
                    f.Description,
                    f.IsActive,
                    f.CreatedAt,
                    f.UpdatedAt))
                .FirstOrDefaultAsync(cancellationToken);

            if (faculty is null)
                return FacultyErrors.NotFound;

            return faculty;
        }
    }

    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/v{v:apiVersion}/faculties/{id:guid}", async (ISender sender, Guid id, CancellationToken cancellationToken) =>
                    {
                        var result = await sender.Send(new Query(id), cancellationToken);
                        return result.ToHttpResult();
                    })
                //.RequirePermission(PermissionCodes.Faculties.Read)
                .Version(app, 1.0)
                .WithName("GetFacultyById")
                .WithTags("Faculties");
        }
    }
}