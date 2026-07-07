using File.Api.Domain.File.Errors;
using System.Security.Cryptography;

namespace File.Api.Features.Files.v1.Upload;

public static class Upload
{
    public sealed record UploadFileRequest(IFormFile File);

    public sealed record Command(IFormFile File) : ICommand<Result<Guid>>;

    public class Handler(IFileRepository fileRepository,
        IFileStorage fileStorage,
        IFilePathGenerator filePathGenerator,
        IUnitOfWork unitOfWork) 
        : ICommandHandler<Command, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (request.File is null)
                return FileErrors.FileIsRequired;

            if (request.File.Length == 0)
                return FileErrors.FileIsEmpty;

            var fileResult = Domain.File.File.Create(
                request.File.FileName,
                request.File.ContentType,
                request.File.Length,
                uploadedBy: null);

            if (fileResult.IsFailure)
                return fileResult.Error;

            var file = fileResult.Data;

            string relativePath = filePathGenerator.GetRelativePath(file.Id, file.FileName);
            var storagePathResult = file.SetStoragePath(relativePath);
            if (storagePathResult.IsFailure)
                return storagePathResult.Error;

            await using var stream = request.File.OpenReadStream();

            string hash = await ComputeSha256Async(stream, cancellationToken);

            var hashResult = file.SetHash(hash);

            if (hashResult.IsFailure)
                return hashResult.Error;

            await fileStorage.SaveAsync(stream, relativePath, cancellationToken);

            try
            {
                await fileRepository.AddAsync(file, cancellationToken);
                await unitOfWork.SaveAsync(cancellationToken);
            }
            catch
            {
                await fileStorage.DeleteAsync(relativePath, CancellationToken.None);
                throw;
            }
            
            return file.Id.Value;
        }

        private static async Task<string> ComputeSha256Async(Stream stream, CancellationToken cancellationToken)
        {
            if (stream.CanSeek)
                stream.Position = 0;

            using var sha256 = SHA256.Create();

            byte[] hashBytes = await sha256.ComputeHashAsync(
                stream,
                cancellationToken);

            if (stream.CanSeek)
                stream.Position = 0;

            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }
    }

    public class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/v{v:apiVersion}/files/upload", async (IFormFile file, ISender sender) =>
            {
                var cmd = new Command(file);
                var result = await sender.Send(cmd);
                return result.ToHttpResult();
            })
            .DisableAntiforgery()
            .Version(app, 1.0)
            .WithName("UploadFile")
            .WithTags("Files");
        }
    }
}