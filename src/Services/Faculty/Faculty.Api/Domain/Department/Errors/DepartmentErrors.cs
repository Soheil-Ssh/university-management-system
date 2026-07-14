namespace Faculty.Api.Domain.Department.Errors;

public static class DepartmentErrors
{
    // General errors
    public static Error NotFound = new("Department.NotFound", $"Department was not found.", ErrorType.NotFound);
    public static readonly Error CannotAssignExpertToInactiveDepartment
        = new("Department.CannotAssignExpertToInactiveDepartment", "A primary expert cannot be assigned to an inactive department.", ErrorType.Conflict);
    public static readonly Error CannotAssignHeadToInactiveDepartment =
        new("Department.CannotAssignHeadToInactiveDepartment", "A head professor cannot be assigned to an inactive department.", ErrorType.Conflict);

    // Name errors
    public static readonly Error NameEmpty = new("Department.Name.Empty", "Department name cannot be empty.", ErrorType.Validation);
    public static readonly Error NameTooLong = new("Department.Name.TooLong", "Department name is too long.", ErrorType.Validation);

    // Short name errors
    public static readonly Error ShortNameTooLong = new("Department.ShortName.TooLong", "Department short name is too long.", ErrorType.Validation);

    // Description errors
    public static readonly Error DescriptionTooLong = new("Department.Description.TooLong", "Department description is too long.", ErrorType.Validation);

    // Internal phone number errors
    public static readonly Error InternalPhoneNumberTooLong = new("Department.InternalPhone.NumberTooLong", "Department internal phone number is too long.", ErrorType.Validation);

    // Office location errors
    public static readonly Error OfficeLocationTooLong = new("Department.OfficeLocation.TooLong", "Department office location is too long.", ErrorType.Validation);
}