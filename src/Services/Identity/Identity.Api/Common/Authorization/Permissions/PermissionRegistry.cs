namespace Identity.Api.Common.Authorization.Permissions;

public static class PermissionRegistry
{
    public static IReadOnlyCollection<PermissionDefinition> All =>
    [
        ..IdentityPermissions.All,
    ];
}