namespace Faculty.Api.Domain.Professor.Events;

public sealed record ProfessorIdentityProvisioningRequestedDomainEvent(Guid ProfessorId,
    string ProfessorCode,
    string NationalCode,
    string FirstName,
    string LastName,
    string Email,
    string MobileNumber) : DomainEvent;