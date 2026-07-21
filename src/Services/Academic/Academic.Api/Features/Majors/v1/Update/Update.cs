using Academic.Application.Features.Majors.Commands.Update;

namespace Academic.Api.Features.Majors.v1.Update;

public static class Update
{
    public sealed record UpdateMajorRequest(Guid DepartmentId, string Name, string? Description);

    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("api/v{v:apiVersion}/majors/{majorId:guid}",
                    async (Guid majorId, UpdateMajorRequest request, ISender sender, CancellationToken cancellationToken) =>
                    {
                        var command = new UpdateMajorCommand(majorId, request.DepartmentId, request.Name, request.Description);
                        var result = await sender.Send(command, cancellationToken);
                        return result.ToHttpResult();
                    })
                //.RequirePermission()
                .Version(app, 1.0)
                .WithName("UpdateMajor")
                .WithTags("Majors");
        }
    }
}