namespace Identity.Api.Domain.Permission.Errors;

public static class PermissionErrors
{
    public static readonly Error NameEmpty = new("Permission.Name.Empty", "Permission name cannot be empty.", ErrorType.Validation);
    public static readonly Error NameTooLong = new("Permission.Name.TooLong", "Permission name is too long.", ErrorType.Validation);
    public static readonly Error CodeEmpty = new("Permission.Code.Empty", "Permission code cannot be empty.", ErrorType.Validation);
    public static readonly Error CodeTooLong = new("Permission.Code.TooLong", "Permission code is too long.", ErrorType.Validation);
}