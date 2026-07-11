using Faculty.Api.Domain.Faculty.ValueObjects;

namespace Faculty.Api.Domain.Faculty;

public interface IFacultyRepository
{
    Task<Faculty?> GetByIdAsync(FacultyId id, CancellationToken cancellationToken = default);
    Task AddAsync(Faculty faculty, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(FacultyCode code, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, FacultyId? excludeId = null, CancellationToken cancellationToken = default);

}