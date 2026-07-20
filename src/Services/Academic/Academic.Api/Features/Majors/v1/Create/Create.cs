using Academic.Application.Features.Majors.Commands.Create;

namespace Academic.Api.Features.Majors.v1.Create;

public static class Create
{
    public sealed record CreateMajorRequest(Guid DepartmentId, string Name, string? Description);

    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/v{v:apiVersion}/majors",
                    async (ISender sender, CreateMajorRequest request, CancellationToken cancellationToken) =>
                    {
                        var command = request.Adapt<CreateMajorCommand>();
                        var result = await sender.Send(command, cancellationToken);
                        return result.ToHttpResult();
                    })
                //.RequirePermission()
                .Version(app, 1.0)
                .WithName("CreateMajor")
                .WithTags("Majors");
        }
    }
}