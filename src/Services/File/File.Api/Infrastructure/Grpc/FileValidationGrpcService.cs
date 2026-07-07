using Grpc.Core;
using SharedKernel.Contracts.Grpc.File.v1;

namespace File.Api.Infrastructure.Grpc;

public class FileValidationGrpcService(FileDbContext dbContext) : FileValidationService.FileValidationServiceBase
{
    public override async Task<FileExistsResponse> Exists(FileExistsRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.FileId, out var fileGuid))
            return new FileExistsResponse { Exists = false };

        var exists = await dbContext.Files.AnyAsync(x => x.Id == new FileId(fileGuid), context.CancellationToken);
        return new FileExistsResponse { Exists = exists };
    }
}