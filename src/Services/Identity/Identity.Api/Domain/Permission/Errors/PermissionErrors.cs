namespace Identity.Api.Domain.Permission.Errors;

public static class PermissionErrors
{
    // General errors
    public static readonly Error NotFound = new("Permission.NotFound", "Permission not found.", ErrorType.Validation);

    // Name
    public static readonly Error NameEmpty = new("Permission.Name.Empty", "Permission name cannot be empty.", ErrorType.Validation);
    public static readonly Error NameTooLong = new("Permission.Name.TooLong", "Permission name is too long.", ErrorType.Validation);

    // Display Name
    public static readonly Error DisplayNameEmpty = new("Permission.DisplayName.Empty", "Permission display name cannot be empty.", ErrorType.Validation);
    public static readonly Error DisplayNameTooLong = new("Permission.DisplayName.TooLong", "Permission display name is too long.", ErrorType.Validation);

    // Code
    public static readonly Error CodeEmpty = new("Permission.Code.Empty", "Permission code cannot be empty.", ErrorType.Validation);
    public static readonly Error CodeTooLong = new("Permission.Code.TooLong", "Permission code is too long.", ErrorType.Validation);
}