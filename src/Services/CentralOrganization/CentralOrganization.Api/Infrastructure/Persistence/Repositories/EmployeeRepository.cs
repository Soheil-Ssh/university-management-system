namespace CentralOrganization.Api.Infrastructure.Persistence.Repositories;

public class EmployeeRepository(CentralOrganizationDbContext context) : IEmployeeRepository
{
    public async Task<Employee?> GetByIdAsync(EmployeeId id, CancellationToken cancellationToken = default)
        => await context.Employees
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task AddAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        await context.Employees.AddAsync(employee, cancellationToken);
    }

    public async Task<bool> IsExistPersonnelCodeAsync(PersonnelCode personnelCode, CancellationToken cancellationToken = default)
        => await context.Employees
            .AnyAsync(e => e.PersonnelCode.Value == personnelCode.Value, cancellationToken);

    public async Task<bool> IsExistNationalCodeAsync(NationalCode nationalCode, CancellationToken cancellationToken = default)
        => await context.Employees
            .AnyAsync(e => e.NationalCode == nationalCode, cancellationToken);

    public async Task<bool> IsExistMobileNumberAsync(MobileNumber mobileNumber, CancellationToken cancellationToken = default)
        => await context.Employees
            .AnyAsync(e => e.MobileNumber == mobileNumber, cancellationToken);

    public async Task<bool> IsExistEmailAsync(Email email, CancellationToken cancellationToken = default)
        => await context.Employees
            .AnyAsync(e => e.Email == email, cancellationToken);

    public async Task<int> GetNextPersonnelCodeAsync(CancellationToken cancellationToken)
    {
        var prefix = "UMS_CO_EMP_";

        var lastCode = await context.Employees
            .AsNoTracking()
            .Where(e => e.PersonnelCode.Value.StartsWith(prefix))
            .OrderByDescending(e => e.PersonnelCode.Value)
            .Select(e => e.PersonnelCode)
            .FirstOrDefaultAsync(cancellationToken);

        if (lastCode is null)
            return 1;

        var sequencePart = lastCode.Value[prefix.Length..];

        if (!int.TryParse(sequencePart, out var lastSequenceNumber))
            return 1;

        return lastSequenceNumber + 1;
    }
}