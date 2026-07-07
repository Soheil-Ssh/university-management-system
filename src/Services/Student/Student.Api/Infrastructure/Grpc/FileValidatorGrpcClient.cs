using Grpc.Core;
using SharedKernel.Contracts.Grpc.File.v1;
using SharedKernel.Domain.Identifiers;
using Student.Api.Application.Abstractions.Errors;

namespace Student.Api.Infrastructure.Grpc;

public class FileValidatorGrpcClient(FileValidationService.FileValidationServiceClient client) : IFileValidatorClient
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
            return FileValidatorClientErrors.Unavailable;
        }
    }
}