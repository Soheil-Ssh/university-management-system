namespace Identity.Api.Infrastructure.Authorization.Permissions;

public static class PermissionRegistry
{
    public static IReadOnlyCollection<PermissionDefinition> All =>
    [
        ..IdentityPermissions.All,
    ];
}