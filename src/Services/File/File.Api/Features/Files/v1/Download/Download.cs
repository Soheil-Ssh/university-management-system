using Microsoft.Net.Http.Headers;

namespace File.Api.Features.Files.v1.Download;

using File.Api.Domain.File.Errors;

public static class Download
{
    public sealed record Response(Stream Content, string FileName, string MimeType, long Size, string? Hash, DateTime UpdatedAt);

    public sealed record Query(Guid Id) : IQuery<Result<Response>>;

    public class Handler(IFileRepository fileRepository, IFileStorage fileStorage) : IQueryHandler<Query, Result<Response>>
    {
        public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var fileId = new FileId(request.Id);

            var file = await fileRepository.GetByIdAsync(fileId, cancellationToken);

            if (file is null)
                return FileErrors.NotFound;

            if (file.Status == FileStatus.Deleted)
                return FileErrors.Deleted;

            if (string.IsNullOrWhiteSpace(file.StoragePath))
                return FileErrors.StoragePathEmpty;

            bool exists = await fileStorage.ExistsAsync(file.StoragePath, cancellationToken);
            if (!exists)
                return FileErrors.PhysicalFileNotFound;

            var stream = await fileStorage.OpenReadAsync(file.StoragePath, cancellationToken);

            return new Response(stream,
                file.FileName.Value,
                file.MimeType,
                file.Size,
                file.Hash?.Value,
                file.UpdatedAt);
        }
    }

    public class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/v{v:apiVersion}/files/{id:guid}/download",
                    async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    {
                        var query = new Query(id);
                        var result = await sender.Send(query, cancellationToken);

                        if (result.IsFailure)
                            return result.ToHttpResult();

                        var file = result.Data;

                        EntityTagHeaderValue? entityTag = null;

                        if (!string.IsNullOrWhiteSpace(file.Hash))
                        {
                            entityTag = new EntityTagHeaderValue($"\"{file.Hash}\"");
                        }

                        var lastModified = new DateTimeOffset(
                            DateTime.SpecifyKind(file.UpdatedAt, DateTimeKind.Utc));

                        return Results.File(
                            file.Content,
                            contentType: file.MimeType,
                            fileDownloadName: file.FileName,
                            lastModified: lastModified,
                            entityTag: entityTag,
                            enableRangeProcessing: true);
                    })
                .Version(app, 1.0)
                .WithName("DownloadFile")
                .WithTags("Files");
        }
    }
}