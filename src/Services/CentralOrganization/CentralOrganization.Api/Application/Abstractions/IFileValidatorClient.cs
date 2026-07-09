using SharedKernel.Domain.Identifiers;

namespace CentralOrganization.Api.Application.Abstractions;

public interface IFileValidatorClient
{
    Task<Result<bool>> ExistsAsync(FileId fileId, CancellationToken cancellationToken);
}