namespace Faculty.Api.Features.Professors.v1.GetById;

public static class GetById
{
    public sealed record DeanFacultyResponse(Guid Id, string Code, string Name);

    public sealed record Response(
        Guid Id,
        string Code,
        string FirstName,
        string LastName,
        string FatherName,
        string FullName,
        string NationalCode,
        string Email,
        string MobileNumber,
        string Specialization,
        AcademicRank AcademicRank,
        ProfessorEmploymentType EmploymentType,
        DateOnly EmploymentStartDate,
        Guid? ProfileImageFileId,
        Guid? IdentityUserId,
        IdentityProvisioningStatus IdentityProvisioningStatus,
        string? IdentityProvisioningFailureReason,
        bool IsActive,
        DeanFacultyResponse? DeanFaculty,
        DateTime CreatedAt,
        DateTime UpdatedAt);

    public sealed record Query(Guid Id) : IQuery<Result<Response>>;

    public sealed class Handler(FacultyDbContext context) : IQueryHandler<Query, Result<Response>>
    {
        public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var professorId = new ProfessorId(request.Id);

            // ReSharper disable once EntityFramework.NPlusOne.IncompleteDataQuery
            var professor = await context.Professors
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == professorId, cancellationToken);
            if (professor is null)
                return ProfessorErrors.NotFound;

            var deanFaculty = await context.Faculties
                .AsNoTracking()
                .Where(f => f.DeanProfessorId == professorId)
                .Select(f => new
                {
                    f.Id,
                    f.Code,
                    f.Name
                })
                .FirstOrDefaultAsync(cancellationToken);

            return new Response(
                professor.Id.Value,
                professor.Code.Value,
                // ReSharper disable once EntityFramework.NPlusOne.IncompleteDataUsage
                professor.FirstName.Value,
                // ReSharper disable once EntityFramework.NPlusOne.IncompleteDataUsage
                professor.LastName.Value,
                // ReSharper disable once EntityFramework.NPlusOne.IncompleteDataUsage
                professor.FatherName.Value,
                professor.FullName,
                // ReSharper disable once EntityFramework.NPlusOne.IncompleteDataUsage
                professor.NationalCode.Value,
                // ReSharper disable once EntityFramework.NPlusOne.IncompleteDataUsage
                professor.Email.Value,
                // ReSharper disable once EntityFramework.NPlusOne.IncompleteDataUsage
                professor.MobileNumber.Value,
                professor.Specialization,
                professor.AcademicRank,
                professor.EmploymentType,
                professor.EmploymentStartDate,
                professor.ProfileImageFileId?.Value,
                professor.IdentityUserId?.Value,
                professor.IdentityProvisioningStatus,
                professor.IdentityProvisioningFailureReason,
                professor.IsActive,
                deanFaculty is null
                    ? null
                    : new DeanFacultyResponse(
                        deanFaculty.Id.Value,
                        deanFaculty.Code.Value,
                        deanFaculty.Name),
                professor.CreatedAt,
                professor.UpdatedAt);
        }

        public sealed class Endpoint : ICarterModule
        {
            public void AddRoutes(IEndpointRouteBuilder app)
            {
                app.MapGet("api/v{v:apiVersion}/professors/{id:guid}",
                        async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                        {
                            var result = await sender.Send(new Query(id), cancellationToken);
                            return result.ToHttpResult();
                        })
                    //.RequirePermission(PermissionCodes.Faculty.ProfessorsRead)
                    .Version(app, 1.0)
                    .WithName("GetProfessorById")
                    .WithTags("Professors");
            }
        }
    }
}