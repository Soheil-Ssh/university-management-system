namespace CentralOrganization.Api.Domain.Unit.Errors;

public static class UnitErrors
{
    // Code
    public static readonly Error CodeAlreadyExists = new("Unit.Code.AlreadyExists", "A unit with the specified code already exists.", ErrorType.Conflict);

    // Name errors
    public static readonly Error NameEmpty = new("Unit.Name.Empty", "Unit name cannot be empty.", ErrorType.Validation);
    public static readonly Error NameTooLong = new("Unit.Name.TooLong", "Unit name is too long.", ErrorType.Validation);

    // Description errors
    public static readonly Error DescriptionEmpty = new("Unit.Description.Empty", "Unit description cannot be empty.", ErrorType.Validation);
    public static readonly Error DescriptionTooLong = new("Unit.Description.TooLong", "Unit description is too long.", ErrorType.Validation);
}