using Academic.Application.Features.Majors.Queries.GetById;

namespace Academic.Api.Features.Majors.v1.GetById;

public static class GetById
{
    public sealed record Response(Guid Id, Guid DepartmentId, string Code, string Name, string? Description, bool IsActive, DateTime CreatedAt, DateTime UpdatedAt);

    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/v{v:apiVersion}/majors/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    {
                        var query = new GetMajorByIdQuery(id);
                        var result = await sender.Send(query, cancellationToken);
                        return result.ToHttpResult<GetMajorByIdDto, Response>();
                    })
                //.RequirePermission()
                .Version(app, 1.0)
                .WithName("GetMajorById")
                .WithTags("Majors");
        }
    }
}