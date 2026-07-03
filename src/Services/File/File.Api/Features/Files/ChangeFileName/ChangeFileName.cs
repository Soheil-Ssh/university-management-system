namespace File.Api.Features.Files.ChangeFileName;

//public static class ChangeFileName
//{
//    public sealed record ChangeFileNameRequest(Guid Id, string NewName);

//    public sealed record Command(Guid Id, string NewName) : ICommand<Result>;

//    public class Handler(FileService service) : ICommandHandler<Command, Result>
//    {
//        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
//        {
//            return await service.ChangeFileNameAsync(new FileId(request.Id), request.NewName, cancellationToken);
//        }
//    }

//    public class Endpoint : ICarterModule
//    {
//        public void AddRoutes(IEndpointRouteBuilder app)
//        {
//            app.MapPut("api/v{v:apiVersion}/files/{id}/name", async (Guid id, ChangeFileNameRequest req, ISender sender) =>
//            {
//                var cmd = new Command(id, req.NewName);
//                var result = await sender.Send(cmd);
//                return result.ToHttpResult();
//            })
//            .Version(app, 1.0)
//            .WithName("ChangeFileName")
//            .WithTags("Files");
//        }
//    }
//}
