namespace SharedKernel.Identity.Permissions;

public static class PermissionCodes
{
    public static class Identity
    {
        // User permissions
        public static readonly string UsersCreate = "identity.users.create";
        public static readonly string UsersRead = "identity.users.read";
        public static readonly string UsersUpdate = "identity.users.update";
        public static readonly string UsersDelete = "identity.users.delete";
        public static readonly string UsersAssignRole = "identity.users.assign_role";
        public static readonly string UsersRemoveRole = "identity.users.remove_role";

        // Role permissions
        public static readonly string RolesCreate = "identity.roles.create";
        public static readonly string RolesRead = "identity.roles.read";
        public static readonly string RolesUpdate = "identity.roles.update";
        public static readonly string RolesDelete = "identity.roles.delete";
        public static readonly string RolesAssignPermission = "identity.roles.assign_permission";
        public static readonly string RolesRemovePermission = "identity.roles.remove_permission";

        // Permission permissions
        public static readonly string PermissionsRead = "identity.permissions.read";
    }
}