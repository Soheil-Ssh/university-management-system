namespace Academic.Domain.Major.Errors;

public static class MajorErrors
{
    // DepartmentId errors
    public static readonly Error DepartmentIdEmpty = new("Major.DepartmentId.Empty", "Department id is required.", ErrorType.Validation);
    public static readonly Error DepartmentAlreadyHasMajor = new("Major.DepartmentAlreadyHasMajor", "The selected department already has a major.", ErrorType.Conflict);

    // Name errors
    public static readonly Error NameEmpty = new("Major.Name.Empty", "Major name is required.", ErrorType.Validation);
    public static readonly Error NameTooLong = new("Major.Name.TooLong", "Major name is too long.", ErrorType.Validation);

    // Description errors
    public static readonly Error DescriptionTooLong = new("Major.Description.TooLong", "Major description is too long.", ErrorType.Validation);
}