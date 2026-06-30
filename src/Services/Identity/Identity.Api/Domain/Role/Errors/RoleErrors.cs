namespace Identity.Api.Domain.Role.Errors;

public static class RoleErrors
{
    // General errors
    public static readonly Error AlreadyExists = new("Role.AlreadyExists", "A role with this name already exists.", ErrorType.Validation);
    public static readonly Error NotFound = new("Role.NotFound", "Role not found.", ErrorType.Validation);

    public static readonly Error SystemRoleCannotBeDeactivated = 
        new("Role.System.CannotDeactivate", "A system role cannot be deactivated.", ErrorType.Validation);
    public static readonly Error SystemRoleNameCannotBeRename = 
        new("Role.System.CannotRename", "Cannot modify name of system role.", ErrorType.Validation);
    public static readonly Error NonSystemRoleCannotBeSynchronized = 
        new("Role.System.OnlySystemRolesCanSynchronize", "Only system roles can be synchronized.", ErrorType.Validation);
    public static readonly Error SystemRolePermissionsCannotBeModified = 
        new("Role.System.CannotModifyPermissions", "Permissions of a system role cannot be modified.", ErrorType.Validation);

    // Role name errors
    public static readonly Error NameEmpty = new("Role.Name.Empty", "Role name cannot be empty.", ErrorType.Validation);
    public static readonly Error NameTooLong = new("Role.Name.TooLong", "Role name is too long.", ErrorType.Validation);
    public static readonly Error NameTooShort = new("Role.Name.TooShort", "Role name is too short.", ErrorType.Validation);

    // Role display name errors
    public static readonly Error DisplayNameEmpty = new("Role.DisplayName.Empty", "Role display name cannot be empty.", ErrorType.Validation);
    public static readonly Error DisplayNameTooLong = new("Role.DisplayName.TooLong", "Role display name is too long.", ErrorType.Validation);
    public static readonly Error DisplayNameTooShort = new("Role.DisplayName.TooShort", "Role display name is too short.", ErrorType.Validation);

    // Role description errors
    public static readonly Error DescriptionEmpty = new("Role.Description.Empty", "Description cannot be empty.", ErrorType.Validation);
    public static readonly Error DescriptionTooLong = new("Role.Description.TooLong", "Description is too long.", ErrorType.Validation);

    // Role permission errors
    public static readonly Error PermissionAlreadyExist = new("Role.Permission.AlreadyExists", "Permission is already assigned to this role.", ErrorType.Validation);
    public static readonly Error PermissionNotFound = new("User.Permission.NotFound", "Permission not found for this user.", ErrorType.Validation);
}