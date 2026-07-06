
namespace CentralOrganization.Api.Domain.Employee;

public interface IEmployeeRepository
{
    Task<Employee?> GetById(EmployeeId id, CancellationToken cancellationToken = default);
    Task AddAsync(Employee employee, CancellationToken cancellationToken = default);
    Task<bool> IsExistPersonnelCodeAsync(PersonnelCode personnelCode, CancellationToken cancellationToken = default);
    Task<bool> IsExistNationalCodeAsync(NationalCode nationalCode, CancellationToken cancellationToken = default);
    Task<bool> IsExistMobileNumberAsync(MobileNumber mobileNumber, CancellationToken cancellationToken = default);
    Task<bool> IsExistEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<int> GetNextPersonnelCodeAsync(CancellationToken cancellationToken);
}