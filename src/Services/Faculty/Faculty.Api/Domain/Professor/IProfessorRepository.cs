namespace Faculty.Api.Domain.Professor;

public interface IProfessorRepository
{
    Task AddAsync(Professor professor, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNationalCodeAsync(NationalCode nationalCode, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByMobileNumberAsync(MobileNumber mobileNumber, CancellationToken cancellationToken = default);
    Task<int> GetNextCodeNumberAsync(CancellationToken cancellationToken = default);
}