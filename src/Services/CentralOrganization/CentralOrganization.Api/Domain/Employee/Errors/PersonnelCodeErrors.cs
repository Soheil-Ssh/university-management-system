namespace CentralOrganization.Api.Domain.Employee.Errors;

public static class PersonnelCodeErrors
{
    public static readonly Error Empty = new("PersonnelCode.Empty", "Personnel code cannot be empty.", ErrorType.Validation);
    public static readonly Error InvalidFormat = new("PersonnelCode.InvalidFormat", "Personnel code format is invalid. Expected format is UMS_EMP_000001.", ErrorType.Validation);
    public static readonly Error NumberOutOfRange = new("PersonnelCode.NumberOutOfRange", "Personnel code number must be between 1 and 9999.", ErrorType.Validation);
}