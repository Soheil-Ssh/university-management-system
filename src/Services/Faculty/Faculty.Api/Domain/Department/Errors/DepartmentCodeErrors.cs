namespace Faculty.Api.Domain.Department.Errors;

public static class DepartmentCodeErrors
{
    public static readonly Error Empty = new("DepartmentCode.Empty", "Department code cannot be empty.", ErrorType.Validation);
    public static readonly Error InvalidFormat =
        new("DepartmentCode.InvalidFormat", "Department code format is invalid. Expected format is UMS_FAC_DEP_0001.", ErrorType.Validation);
    public static readonly Error NumberOutOfRange = new("DepartmentCode.NumberOutOfRange", "Department code number must be between 1 and 9999.", ErrorType.Validation);
}