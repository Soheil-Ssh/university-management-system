using File.Api.Application.Protos;
using Grpc.Core;
using SharedKernel.Domain.Identifiers;
using Student.Api.Application.Abstractions.Errors;

namespace Student.Api.Infrastructure.FileServices;

public class GrpcFileValidator(FileValidationService.FileValidationServiceClient client) : IFileValidator
{
    public async Task<Result<bool>> ExistsAsync(FileId fileId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await client.ExistsAsync(
                new FileExistsRequest { FileId = fileId.Value.ToString() },
                cancellationToken: cancellationToken);

            return response.Exists;
        }
        catch (RpcException exception) when (exception.StatusCode is StatusCode.Unavailable
                or StatusCode.DeadlineExceeded
                or StatusCode.Internal)
        {
            return FileValidator.Unavailable;
        }
    }
}