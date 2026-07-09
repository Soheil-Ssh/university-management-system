namespace CentralOrganization.Api.Domain.Employee.Events;

public sealed record EmployeeIdentityProvisioningRequestedDomainEvent(Guid EmployeeId,
    string PersonnelCode,
    string NationalCode,
    string FirstName,
    string LastName,
    string Email,
    string MobileNumber) : DomainEvent;