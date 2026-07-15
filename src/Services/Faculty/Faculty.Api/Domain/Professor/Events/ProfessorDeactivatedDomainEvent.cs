namespace Faculty.Api.Domain.Professor.Events;

public sealed record ProfessorDeactivatedDomainEvent(ProfessorId ProfessorId) : DomainEvent;