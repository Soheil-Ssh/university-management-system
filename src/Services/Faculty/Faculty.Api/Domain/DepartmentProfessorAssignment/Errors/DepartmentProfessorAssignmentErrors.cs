namespace Faculty.Api.Domain.DepartmentProfessorAssignment.Errors;

public static class DepartmentProfessorAssignmentErrors
{
    // General errors
    public static readonly Error AlreadyUnassigned = 
        new("DepartmentProfessorAssignment.AlreadyUnassigned", "The professor has already been unassigned from the department.", ErrorType.Conflict);
    public static readonly Error InvalidUnassignedAt = 
        new("DepartmentProfessorAssignment.InvalidUnassignedAt", "The unassignment date cannot be earlier than the assignment date.", ErrorType.Validation);
    public static readonly Error AlreadyExists = 
        new("DepartmentProfessorAssignment.AlreadyExists", "The professor is already assigned to this department.", ErrorType.Conflict);

    // Department errors
    public static readonly Error DepartmentRequired = new("DepartmentProfessorAssignment.Department.Required", "Department is required.", ErrorType.Validation);

    // Professor errors
    public static readonly Error ProfessorRequired = new("DepartmentProfessorAssignment.Professor.Required", "Professor is required.", ErrorType.Validation);
}