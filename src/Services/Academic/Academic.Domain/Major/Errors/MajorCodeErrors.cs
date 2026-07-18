namespace Academic.Domain.Major.Errors;

public static class MajorCodeErrors
{
    public static readonly Error Empty = new("MajorCode.Empty", "Major code cannot be empty.", ErrorType.Validation);
    public static readonly Error InvalidFormat = new("MajorCode.InvalidFormat", "MajorMajor code format is invalid. Expected format is UMS_AC_MAJ_0001.", ErrorType.Validation);
    public static readonly Error NumberOutOfRange = new("MajorCode.NumberOutOfRange", "Major code number must be between 1 and 9999.", ErrorType.Validation);
}