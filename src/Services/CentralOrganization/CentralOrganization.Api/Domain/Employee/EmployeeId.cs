namespace CentralOrganization.Api.Domain.Employee;

public sealed record EmployeeId(Guid Value)
{
    public static EmployeeId New() => new(Guid.NewGuid());
}