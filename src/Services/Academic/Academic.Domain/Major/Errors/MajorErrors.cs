namespace Academic.Domain.Major.Errors;

public static class MajorErrors
{
    // Global errors
    public static readonly Error NotFound = new("Major.NotFound", "The major was not found.", ErrorType.NotFound);

    // DepartmentId errors
    public static readonly Error DepartmentIdEmpty = new("Major.DepartmentId.Empty", "Department id is required.", ErrorType.Validation);
    public static readonly Error DepartmentAlreadyHasMajor = new("Major.Department.AlreadyHasMajor", "The selected department already has a major.", ErrorType.Conflict);
    public static readonly Error DepartmentNotFound = new("Major.Department.NotFound", "The selected department was not found.", ErrorType.NotFound);
    public static readonly Error DepartmentInactive = new("Major.Department.Inactive", "The selected department is inactive.", ErrorType.Conflict);
    public static readonly Error DepartmentValidationFailed = new("Major.Department.ValidationFailed", "The department eligibility could not be validated.", ErrorType.Failure);

    // Name errors
    public static readonly Error NameEmpty = new("Major.Name.Empty", "Major name is required.", ErrorType.Validation);
    public static readonly Error NameTooLong = new("Major.Name.TooLong", "Major name is too long.", ErrorType.Validation);

    // Description errors
    public static readonly Error DescriptionTooLong = new("Major.Description.TooLong", "Major description is too long.", ErrorType.Validation);
}