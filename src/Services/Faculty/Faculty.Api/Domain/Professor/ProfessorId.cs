namespace Faculty.Api.Domain.Professor;

public sealed record ProfessorId(Guid Value)
{
    public static ProfessorId New() => new(Guid.NewGuid());
}