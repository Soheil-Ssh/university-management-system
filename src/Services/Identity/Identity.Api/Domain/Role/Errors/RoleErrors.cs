namespace Identity.Api.Domain.Role.Errors;

public static class RoleErrors
{
    // General role errors
    public static readonly Error AlreadyExists = new("Role.AlreadyExists", "A role with this name already exists.", ErrorType.Validation);
    public static readonly Error NotFound = new("Role.NotFound", "Role not found.", ErrorType.Validation);
    public static readonly Error SystemRoleCannotBeModified = new("Role.SystemRoleCannotBeModified", "Cannot modify a system role.", ErrorType.Validation);

    // Role name errors
    public static readonly Error NameEmpty = new("Role.Name.Empty", "Role name cannot be empty.", ErrorType.Validation);
    public static readonly Error NameTooLong = new("Role.Name.TooLong", "Role name is too long.", ErrorType.Validation);
    public static readonly Error NameTooShort = new("Role.Name.TooShort", "Role name is too short.", ErrorType.Validation);

    // Role description errors
    public static readonly Error DescriptionEmpty = new("Role.Description.Empty", "Description cannot be empty.", ErrorType.Validation);
    public static readonly Error DescriptionTooLong = new("Role.Description.TooLong", "Description is too long.", ErrorType.Validation);
    public static readonly Error PermissionAlreadyExist = new("User.Permission.AlreadyExist", "Permission already exists for this user.", ErrorType.Validation);
    public static readonly Error PermissionNotFound = new("User.Permission.NotFound", "Permission not found for this user.", ErrorType.Validation);
}