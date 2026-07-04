using SharedKernel.Domain.Identifiers;

namespace Student.Api.Application.Abstractions;

public interface IFileValidator
{
    Task<Result<bool>> ExistsAsync(FileId fileId, CancellationToken cancellationToken);
}