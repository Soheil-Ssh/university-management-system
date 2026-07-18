namespace Faculty.Api.Domain.DepartmentProfessorAssignment;

public sealed record DepartmentProfessorAssignmentId(Guid Value)
{
    public static DepartmentProfessorAssignmentId New() => new(Guid.NewGuid());
}