using File.Api.Domain.File.Errors;

namespace File.Api.Features.Files.Upload;

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

            await using var stream = request.File.OpenReadStream();

            await fileStorage.SaveAsync(stream, relativePath, cancellationToken);

            await fileRepository.AddAsync(file, cancellationToken);
            await unitOfWork.SaveAsync(cancellationToken);
            
            return file.Id.Value;
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