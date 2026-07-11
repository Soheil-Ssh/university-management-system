using SharedKernel.Domain.Error;

namespace Faculty.Api.Domain.Faculty.Errors;

public static class FacultyCodeErrors
{
    public static readonly Error Empty = new("UnitCode.Empty", "Unit code cannot be empty.", ErrorType.Validation);
    public static readonly Error InvalidFormat = new("UnitCode.InvalidFormat", "Unit code format is invalid. Expected format is UMS_CO_0001.", ErrorType.Validation);
    public static readonly Error NumberOutOfRange = new("UnitCode.NumberOutOfRange", "Unit code number must be between 1 and 9999.", ErrorType.Validation);
}