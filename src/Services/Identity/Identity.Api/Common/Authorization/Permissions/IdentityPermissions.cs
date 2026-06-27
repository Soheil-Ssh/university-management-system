namespace Identity.Api.Common.Authorization.Permissions;

public static class IdentityPermissions
{
    // User permissions
    public static readonly PermissionDefinition UsersCreate = new("Create users", "identity.users.create", PermissionCategories.Identity);
    public static readonly PermissionDefinition UsersRead = new("View users", "identity.users.read", PermissionCategories.Identity);
    public static readonly PermissionDefinition UsersUpdate = new("Update users", "identity.users.update", PermissionCategories.Identity);
    public static readonly PermissionDefinition UsersDelete = new("Delete users", "identity.users.delete", PermissionCategories.Identity);
    public static readonly PermissionDefinition UsersAssignRole = new("Assign role to user", "identity.users.assign_role", PermissionCategories.Identity);
    public static readonly PermissionDefinition UsersRemoveRole = new("identity.users.remove_role", "Remove role from user", PermissionCategories.Identity);

    // Role permissions
    public static readonly PermissionDefinition RolesCreate = new("Create roles", "identity.roles.create", PermissionCategories.Identity);
    public static readonly PermissionDefinition RolesRead = new("View roles", "identity.roles.read", PermissionCategories.Identity);
    public static readonly PermissionDefinition RolesUpdate = new("Update roles", "identity.roles.update", PermissionCategories.Identity);
    public static readonly PermissionDefinition RolesDelete = new("Delete roles", "identity.roles.delete", PermissionCategories.Identity);
    public static readonly PermissionDefinition RolesAssignPermission = new("Assign permission to role", "identity.roles.assign_permission", PermissionCategories.Identity);
    public static readonly PermissionDefinition RolesRemovePermission = new("Remove permission from role", "identity.roles.remove_permission", PermissionCategories.Identity);

    // Permission permissions
    public static readonly PermissionDefinition PermissionsCreate = new("Create permissions", "identity.permissions.create", PermissionCategories.Identity);
    public static readonly PermissionDefinition PermissionsRead = new("View permissions", "identity.permissions.read", PermissionCategories.Identity);
    public static readonly PermissionDefinition PermissionsUpdate = new("Update permissions", "identity.permissions.update", PermissionCategories.Identity);
    public static readonly PermissionDefinition PermissionsDelete = new("Delete permissions", "identity.permissions.delete", PermissionCategories.Identity);


    public static IEnumerable<PermissionDefinition> All =>
    [
        // User permissions
        UsersCreate,
        UsersRead,
        UsersUpdate,
        UsersDelete,
        UsersAssignRole,
        UsersRemoveRole,

        // Role permissions
        RolesCreate,
        RolesRead,
        RolesUpdate,
        RolesDelete,
        RolesAssignPermission,
        RolesRemovePermission,

        // Permission permissions
        PermissionsCreate,
        PermissionsRead,
        PermissionsUpdate,
        PermissionsDelete
    ];
}