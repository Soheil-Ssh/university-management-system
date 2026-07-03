using SharedKernel.Domain.Identifiers;

namespace File.Api.Domain.File;

public interface IFileRepository
{
    Task<File?> GetByIdAsync(FileId id, CancellationToken cancellationToken = default);
    Task AddAsync(File file, CancellationToken cancellationToken = default);
}
