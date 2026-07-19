namespace Academic.Domain.Course.Errors;

public static class CourseCodeErrors
{
    public static readonly Error Empty = new("MajorCode.Empty", "Major code cannot be empty.", ErrorType.Validation);
    public static readonly Error InvalidFormat = new("MajorCode.InvalidFormat", "MajorMajor code format is invalid. Expected format is UMS_AC_CRS_000001.", ErrorType.Validation);
    public static readonly Error NumberOutOfRange = new("MajorCode.NumberOutOfRange", "Major code number must be between 1 and 999999.", ErrorType.Validation);
}