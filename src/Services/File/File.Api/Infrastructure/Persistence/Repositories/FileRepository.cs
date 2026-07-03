using SharedKernel.Domain.Identifiers;
using Microsoft.EntityFrameworkCore;

namespace File.Api.Infrastructure.Persistence.Repositories;

public class FileRepository(FileDbContext context) : IFileRepository
{
    public async Task<Domain.File.File?> GetByIdAsync(FileId id, CancellationToken cancellationToken = default)
        => await context.Files.FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

    public async Task AddAsync(Domain.File.File file, CancellationToken cancellationToken = default)
    {
        await context.Files.AddAsync(file, cancellationToken);
    }
}
