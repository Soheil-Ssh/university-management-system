using SharedKernel.Abstractions.Pagination;
using SharedKernel.Persistence.Extensions;

namespace Faculty.Api.Features.Professors.v1.GetAll;

public static class GetAll
{
    public enum ProfessorSortBy
    {
        Code = 1,
        FullName = 2,
        Specialization = 3,
        AcademicRank = 4,
        EmploymentType = 5,
        CreatedAt = 6
    }

    public enum SortDirection
    {
        Asc = 1,
        Desc = 2
    }

    public sealed record GetAllProfessorsRequest(string? Code,
        string? Name,
        string? Specialization,
        AcademicRank? AcademicRank,
        ProfessorEmploymentType? EmploymentType,
        IdentityProvisioningStatus? IdentityProvisioningStatus,
        bool? IsActive,
        DateTime? FromCreatedAt,
        DateTime? ToCreatedAt,
        ProfessorSortBy SortBy = ProfessorSortBy.CreatedAt,
        SortDirection SortDirection = SortDirection.Desc,
        int Page = 1,
        int PageSize = 20);

    public sealed record Response(Guid Id,
        string Code,
        string FullName,
        string NationalCode,
        string Specialization,
        AcademicRank AcademicRank,
        ProfessorEmploymentType EmploymentType,
        IdentityProvisioningStatus IdentityProvisioningStatus,
        bool IsActive,
        DateTime CreatedAt);

    public sealed record Query(string? Code,
        string? Name,
        string? Specialization,
        AcademicRank? AcademicRank,
        ProfessorEmploymentType? EmploymentType,
        IdentityProvisioningStatus? IdentityProvisioningStatus,
        bool? IsActive,
        DateTime? FromCreatedAt,
        DateTime? ToCreatedAt,
        ProfessorSortBy SortBy = ProfessorSortBy.CreatedAt,
        SortDirection SortDirection = SortDirection.Desc,
        int Page = 1,
        int PageSize = 20)
        : IQuery<Result<PagedResult<Response>>>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Code).MaximumLength(50);
            RuleFor(x => x.Name).MaximumLength(100);
            RuleFor(x => x.Specialization).MaximumLength(150);
            RuleFor(x => x.AcademicRank).Must(value => !value.HasValue || Enum.IsDefined(value.Value)).WithMessage("AcademicRank is invalid.");
            RuleFor(x => x.EmploymentType).Must(value => !value.HasValue || Enum.IsDefined(value.Value)).WithMessage("EmploymentType is invalid.");
            RuleFor(x => x.IdentityProvisioningStatus).Must(value => !value.HasValue || Enum.IsDefined(value.Value)).WithMessage("IdentityProvisioningStatus is invalid.");
            RuleFor(x => x.SortBy).IsInEnum();
            RuleFor(x => x.SortDirection).IsInEnum();
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
            RuleFor(x => x)
                .Must(x => !x.FromCreatedAt.HasValue ||
                           !x.ToCreatedAt.HasValue ||
                           x.FromCreatedAt.Value <= x.ToCreatedAt.Value)
                .WithMessage("FromCreatedAt cannot be greater than ToCreatedAt.");
        }
    }

    public sealed class Handler(FacultyDbContext context) : IQueryHandler<Query, Result<PagedResult<Response>>>
    {
        public async Task<Result<PagedResult<Response>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = context.Professors.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                var codeResult = ProfessorCode.FromString(request.Code);
                if (codeResult.IsFailure)
                    return codeResult.Error;

                query = query.Where(professor => professor.Code == codeResult.Data);
            }

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var name = request.Name;
                query = query.Where(p => EF.Functions.ILike(p.FirstName.Value, $"%{name}%") ||
                                         EF.Functions.ILike(p.LastName.Value, $"%{name}%"));
            }

            if (!string.IsNullOrWhiteSpace(request.Specialization))
            {
                var specialization = request.Specialization.Trim();
                query = query.Where(p => EF.Functions.ILike(p.Specialization, $"%{specialization}%"));
            }

            if (request.AcademicRank.HasValue)
                query = query.Where(p => p.AcademicRank == request.AcademicRank.Value);

            if (request.EmploymentType.HasValue)
                query = query.Where(p => p.EmploymentType == request.EmploymentType.Value);

            if (request.IdentityProvisioningStatus.HasValue)
                query = query.Where(p => p.IdentityProvisioningStatus == request.IdentityProvisioningStatus.Value);

            if (request.IsActive.HasValue)
                query = query.Where(p => p.IsActive == request.IsActive.Value);

            if (request.FromCreatedAt.HasValue)
                query = query.Where(p => p.CreatedAt >= request.FromCreatedAt.Value);

            if (request.ToCreatedAt.HasValue)
                query = query.Where(p => p.CreatedAt <= request.ToCreatedAt.Value);

            query = ApplySorting(query, request.SortBy, request.SortDirection);

            return await query
                .Select(p => new Response(
                    p.Id.Value,
                    p.Code.Value,
                    p.NationalCode.Value,
                    p.FullName,
                    p.Specialization,
                    p.AcademicRank,
                    p.EmploymentType,
                    p.IdentityProvisioningStatus,
                    p.IsActive,
                    p.CreatedAt))
                .ToPagedResultAsync(request.Page, request.PageSize, cancellationToken);
        }

        private static IQueryable<Professor> ApplySorting(IQueryable<Professor> query, ProfessorSortBy sortBy, SortDirection direction)
        {
            return (sortBy, direction) switch
            {
                (ProfessorSortBy.Code, SortDirection.Asc) =>
                    query.OrderBy(professor => professor.Code).ThenBy(professor => professor.Id),

                (ProfessorSortBy.Code, SortDirection.Desc) =>
                    query.OrderByDescending(professor => professor.Code).ThenBy(professor => professor.Id),

                (ProfessorSortBy.FullName, SortDirection.Asc) =>
                    query.OrderBy(professor => professor.LastName).ThenBy(professor => professor.FirstName).ThenBy(professor => professor.Id),

                (ProfessorSortBy.FullName, SortDirection.Desc) =>
                    query.OrderByDescending(professor => professor.LastName).ThenByDescending(professor => professor.FirstName).ThenBy(professor => professor.Id),

                (ProfessorSortBy.Specialization, SortDirection.Asc) =>
                    query.OrderBy(professor => professor.Specialization).ThenBy(professor => professor.Id),

                (ProfessorSortBy.Specialization, SortDirection.Desc) =>
                    query.OrderByDescending(professor => professor.Specialization).ThenBy(professor => professor.Id),

                (ProfessorSortBy.AcademicRank, SortDirection.Asc) =>
                    query.OrderBy(professor => professor.AcademicRank).ThenBy(professor => professor.Id),

                (ProfessorSortBy.AcademicRank, SortDirection.Desc) =>
                    query.OrderByDescending(professor => professor.AcademicRank).ThenBy(professor => professor.Id),

                (ProfessorSortBy.EmploymentType, SortDirection.Asc) =>
                    query.OrderBy(professor => professor.EmploymentType)
                        .ThenBy(professor => professor.Id),

                (ProfessorSortBy.EmploymentType, SortDirection.Desc) =>
                    query.OrderByDescending(professor => professor.EmploymentType).ThenBy(professor => professor.Id),

                (ProfessorSortBy.CreatedAt, SortDirection.Asc) =>
                    query.OrderBy(professor => professor.CreatedAt).ThenBy(professor => professor.Id),

                _ => query.OrderByDescending(professor => professor.CreatedAt)
                    .ThenBy(professor => professor.Id)
            };
        }

        public sealed class Endpoint : ICarterModule
        {
            public void AddRoutes(IEndpointRouteBuilder app)
            {
                app.MapGet("api/v{v:apiVersion}/professors",
                        async ([AsParameters] GetAllProfessorsRequest request, ISender sender, CancellationToken cancellationToken) =>
                        {
                            var query = request.Adapt<Query>();
                            var result = await sender.Send(query, cancellationToken);
                            return result.ToHttpResult();
                        })
                    //.RequirePermission(PermissionCodes.Faculty.ProfessorsRead)
                    .Version(app, 1.0)
                    .WithName("GetAllProfessors")
                    .WithTags("Professors");
            }
        }
    }
}