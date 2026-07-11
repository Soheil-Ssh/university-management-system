namespace Faculty.Api.Domain.Faculty.Errors;

public static class FacultyCodeErrors
{
    public static readonly Error Empty = new("FacultyCode.Empty", "Faculty code cannot be empty.", ErrorType.Validation);
    public static readonly Error InvalidFormat = 
        new("FacultyCode.InvalidFormat", "Faculty code format is invalid. Expected format is UMS_FAC_FACULTY_0001.", ErrorType.Validation);
    public static readonly Error NumberOutOfRange = new("FacultyCode.NumberOutOfRange", "Faculty code number must be between 1 and 9999.", ErrorType.Validation);
}