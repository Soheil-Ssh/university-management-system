namespace SharedKernel.Contracts.Integration.Events.Faculty.Professor.v1;

public record ProfessorIdentityProvisioningRequestedIntegrationEvent(Guid ProfessorId,
    string NationalCode,
    string FirstName,
    string LastName,
    string Email,
    string MobileNumber) : IntegrationEvent;