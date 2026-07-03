//using File.Api.Application.Services;

//namespace File.Api.Features.Files.Download;

//public static class Download
//{
//    public sealed record DownloadFileRequest(Guid Id);

//    public sealed record Query(Guid Id) : IQuery<Result<FileDownloadResponse>>;

//    public sealed record FileDownloadResponse(Stream Content, string FileName, string ContentType);

//    public class Handler(FileService service) : IQueryHandler<Query, Result<FileDownloadResponse>>
//    {
//        public async Task<Result<FileDownloadResponse>> Handle(Query request, CancellationToken cancellationToken)
//        {
//            var result = await service.DownloadAsync(new FileId(request.Id), cancellationToken);
//            if (result.IsFailure) return result.Error;

//            // TODO: retrieve file metadata for file name and content type
//            return new FileDownloadResponse(result.Data, "file", "application/octet-stream");
//        }
//    }

//    public class Endpoint : ICarterModule
//    {
//        public void AddRoutes(IEndpointRouteBuilder app)
//        {
//            app.MapGet("api/v{v:apiVersion}/files/{id}", async (Guid id, ISender sender) =>
//            {
//                var query = new Query(id);
//                var result = await sender.Send(query);
//                if (result.IsFailure) return result.ToHttpResult();

//                var resp = result.Data;
//                return Results.File(resp.Content, resp.ContentType, resp.FileName);
//            })
//            .Version(app, 1.0)
//            .WithName("DownloadFile")
//            .WithTags("Files");
//        }
//    }
//}
