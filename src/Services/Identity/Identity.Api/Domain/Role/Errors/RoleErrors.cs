namespace Identity.Api.Domain.Role.Errors;

public static class RoleErrors
{
    public static readonly Error NameEmpty = new("Role.Name.Empty", "Role name cannot be empty.", ErrorType.Validation);
    public static readonly Error NameTooLong = new("Role.Name.TooLong", "Role name is too long.", ErrorType.Validation);
    public static readonly Error NameTooShort = new("Role.Name.TooShort", "Role name is too short.", ErrorType.Validation);
    public static readonly Error DescriptionTooLong = new("Role.Description.TooLong", "Description is too long.", ErrorType.Validation);
    public static readonly Error PermissionAlreadyExist = new("User.Permission.AlreadyExist", "Permission already exists for this user.", ErrorType.Validation);
    public static readonly Error PermissionNotFound = new("User.Permission.NotFound", "Permission not found for this user.", ErrorType.Validation);
}