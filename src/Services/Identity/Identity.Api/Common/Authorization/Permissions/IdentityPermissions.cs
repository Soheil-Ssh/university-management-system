namespace Identity.Api.Common.Authorization.Permissions;

public static class IdentityPermissions
{
    // User permissions
    public static readonly PermissionDefinition UsersCreate =
        new("Create users", "ایجاد کاربر", "identity.users.create", PermissionCategories.Identity);

    public static readonly PermissionDefinition UsersRead =
        new("View users", "مشاهده کاربران", "identity.users.read", PermissionCategories.Identity);

    public static readonly PermissionDefinition UsersUpdate =
        new("Update users", "ویرایش کاربر", "identity.users.update", PermissionCategories.Identity);

    public static readonly PermissionDefinition UsersDelete =
        new("Delete users", "حذف کاربر", "identity.users.delete", PermissionCategories.Identity);

    public static readonly PermissionDefinition UsersAssignRole =
        new("Assign role to user", "اختصاص نقش به کاربر", "identity.users.assign_role", PermissionCategories.Identity);

    public static readonly PermissionDefinition UsersRemoveRole =
        new("Remove role from user", "حذف نقش از کاربر", "identity.users.remove_role", PermissionCategories.Identity);


    // Role permissions
    public static readonly PermissionDefinition RolesCreate =
        new("Create roles", "ایجاد نقش", "identity.roles.create", PermissionCategories.Identity);

    public static readonly PermissionDefinition RolesRead =
        new("View roles", "مشاهده نقش‌ها", "identity.roles.read", PermissionCategories.Identity);

    public static readonly PermissionDefinition RolesUpdate =
        new("Update roles", "ویرایش نقش", "identity.roles.update", PermissionCategories.Identity);

    public static readonly PermissionDefinition RolesDelete =
        new("Delete roles", "حذف نقش", "identity.roles.delete", PermissionCategories.Identity);

    public static readonly PermissionDefinition RolesAssignPermission =
        new("Assign permission to role", "اختصاص مجوز به نقش", "identity.roles.assign_permission", PermissionCategories.Identity);

    public static readonly PermissionDefinition RolesRemovePermission =
        new("Remove permission from role", "حذف مجوز از نقش", "identity.roles.remove_permission", PermissionCategories.Identity);


    // Permission permissions
    public static readonly PermissionDefinition PermissionsRead =
        new("View permissions", "مشاهده مجوزها", "identity.permissions.read", PermissionCategories.Identity);

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
        PermissionsRead,
    ];
}