namespace Faculty.Api.Domain.DepartmentProfessorAssignment;

public sealed class DepartmentProfessorAssignment : AggregateRoot<DepartmentProfessorAssignmentId>
{
    public DepartmentId DepartmentId { get; private set; }
    public ProfessorId ProfessorId { get; private set; }
    public DateTime AssignedAt { get; private set; }
    public DateTime? UnassignedAt { get; private set; }
    public bool IsActive => UnassignedAt is null;

#pragma warning disable CS8618
    private DepartmentProfessorAssignment() { }
#pragma warning restore CS8618

    private DepartmentProfessorAssignment(
        DepartmentProfessorAssignmentId id,
        DepartmentId departmentId,
        ProfessorId professorId,
        DateTime assignedAt) : base(id)
    {
        DepartmentId = departmentId;
        ProfessorId = professorId;
        AssignedAt = assignedAt;
    }

    public static Result<DepartmentProfessorAssignment> Create(DepartmentId departmentId, ProfessorId professorId)
    {
        if (departmentId.Value == Guid.Empty)
            return DepartmentProfessorAssignmentErrors.DepartmentRequired;

        if (professorId.Value == Guid.Empty)
            return DepartmentProfessorAssignmentErrors.ProfessorRequired;

        return new DepartmentProfessorAssignment(
            DepartmentProfessorAssignmentId.New(),
            departmentId,
            professorId,
            DateTime.UtcNow);
    }

    public Result Unassign(DateTime unassignedAtUtc)
    {
        if (!IsActive)
            return DepartmentProfessorAssignmentErrors.AlreadyUnassigned;

        if (unassignedAtUtc < AssignedAt)
            return DepartmentProfessorAssignmentErrors.InvalidUnassignedAt;

        UnassignedAt = unassignedAtUtc;

        return Result.Success();
    }
}