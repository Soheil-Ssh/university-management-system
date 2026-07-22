using Academic.Application.Features.Majors.Queries.GetAll;

namespace Academic.Api.Features.Majors.v1.GetAll;

public class GetAll
{
    public sealed record GetAllMajorsRequest(
        Guid? DepartmentId,
        string? Code,
        string? Name,
        bool? IsActive,
        DateTime? FromCreatedAt,
        DateTime? ToCreatedAt,
        DateTime? FromUpdatedAt,
        DateTime? ToUpdatedAt,
        MajorSortBy SortBy = MajorSortBy.CreatedAt,
        SortDirection SortDirection = SortDirection.Desc,
        int Page = 1,
        int PageSize = 20);

    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/v{v:apiVersion}/majors",
                    async ([AsParameters] GetAllMajorsRequest request, ISender sender, CancellationToken cancellationToken) =>
                    {
                        var query = request.Adapt<GetAllMajorsQuery>();
                        var result = await sender.Send(query, cancellationToken);
                        return result.ToHttpResult();
                    })
                //.RequirePermission()
                .Version(app, 1.0)
                .WithName("GetAllMajors")
                .WithTags("Majors");
        }
    }
}