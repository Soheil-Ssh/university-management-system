using File.Api.Domain.File.Errors;

namespace File.Api.Features.Files.v1.ChangeFileName;

public static class ChangeFileName
{
    public sealed record ChangeFileNameRequest(string NewNameWithoutExtension);

    public sealed record Response(Guid Id, string FileName, string NameWithoutExtension, string Extension, DateTime UpdatedAt);

    public sealed record Command(Guid Id, string NewNameWithoutExtension) : ICommand<Result<Response>>;

    public class Handler(IFileRepository fileRepository, IUnitOfWork unitOfWork)
        : ICommandHandler<Command, Result<Response>>
    {
        public async Task<Result<Response>> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            var fileId = new FileId(request.Id);

            var file = await fileRepository.GetByIdAsync(fileId, cancellationToken);

            if (file is null)
                return FileErrors.NotFound;

            var updateResult = file.UpdateNameWithoutExtension(request.NewNameWithoutExtension);

            if (updateResult.IsFailure)
                return updateResult.Error;

            await unitOfWork.SaveAsync(cancellationToken);

            return new Response(file.Id.Value,
                file.FileName.Value,
                file.FileName.NameWithoutExtension,
                file.FileName.Extension,
                file.UpdatedAt);
        }
    }

    public class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("api/v{v:apiVersion}/files/{id:guid}/file-name", 
                    async (Guid id, ChangeFileNameRequest request, ISender sender, CancellationToken cancellationToken) =>
                {
                    var command = new Command(id, request.NewNameWithoutExtension);
                    var result = await sender.Send(command, cancellationToken);
                    return result.ToHttpResult();
                })
            .Version(app, 1.0)
            .WithName("ChangeFileName")
            .WithTags("Files");
        }
    }
}