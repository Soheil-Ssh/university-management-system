using SharedKernel.Identity.Permissions;

namespace Identity.Api.Infrastructure.Authorization.Permissions;

public class SystemPermissionsCatalog
{
    public static IReadOnlyCollection<PermissionDefinition> All =>
    [
        ..IdentityPermissions.All,
    ];

    #region Identity permissions

    public static class IdentityPermissions
    {
        // User permissions
        public static readonly PermissionDefinition UsersCreate =
            new("Create users", "ایجاد کاربر", PermissionCodes.Identity.UsersCreate, PermissionCategories.Identity);

        public static readonly PermissionDefinition UsersRead =
            new("View users", "مشاهده کاربران", PermissionCodes.Identity.UsersRead, PermissionCategories.Identity);

        public static readonly PermissionDefinition UsersUpdate =
            new("Update users", "ویرایش کاربر", PermissionCodes.Identity.UsersUpdate, PermissionCategories.Identity);

        public static readonly PermissionDefinition UsersDelete =
            new("Delete users", "حذف کاربر", PermissionCodes.Identity.UsersDelete, PermissionCategories.Identity);

        public static readonly PermissionDefinition UsersAssignRole =
            new("Assign role to user", "اختصاص نقش به کاربر", PermissionCodes.Identity.UsersAssignRole, PermissionCategories.Identity);

        public static readonly PermissionDefinition UsersRemoveRole =
            new("Remove role from user", "حذف نقش از کاربر", PermissionCodes.Identity.UsersRemoveRole, PermissionCategories.Identity);


        // Role permissions
        public static readonly PermissionDefinition RolesCreate =
            new("Create roles", "ایجاد نقش", PermissionCodes.Identity.RolesCreate, PermissionCategories.Identity);

        public static readonly PermissionDefinition RolesRead =
            new("View roles", "مشاهده نقش‌ها", PermissionCodes.Identity.RolesRead, PermissionCategories.Identity);

        public static readonly PermissionDefinition RolesUpdate =
            new("Update roles", "ویرایش نقش", PermissionCodes.Identity.RolesUpdate, PermissionCategories.Identity);

        public static readonly PermissionDefinition RolesDelete =
            new("Delete roles", "حذف نقش", PermissionCodes.Identity.RolesDelete, PermissionCategories.Identity);

        public static readonly PermissionDefinition RolesAssignPermission =
            new("Assign permission to role", PermissionCodes.Identity.RolesAssignPermission, "identity.roles.assign_permission", PermissionCategories.Identity);

        public static readonly PermissionDefinition RolesRemovePermission =
            new("Remove permission from role", PermissionCodes.Identity.RolesRemovePermission, "identity.roles.remove_permission", PermissionCategories.Identity);


        // Permission permissions
        public static readonly PermissionDefinition PermissionsRead =
            new("View permissions", PermissionCodes.Identity.PermissionsRead, "identity.permissions.read", PermissionCategories.Identity);

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

    #endregion
}

