using Identity.Api.Infrastructure.Authorization.Roles;

namespace Identity.Api.Infrastructure.Authorization.RolePermissions;

public static class RolePermissionsCatalog
{
    public static readonly Dictionary<string, IReadOnlyCollection<string>> All =
        new()
        {
            [SystemRolesCatalog.SuperAdmin.Name] = [..SystemPermissionsCatalog.All.Select(x => x.Code)],

            [SystemRolesCatalog.UniversityPresident.Name] = [.. SystemPermissionsCatalog.All.Select(x => x.Code)],

            [SystemRolesCatalog.Professor.Name] = [],

            [SystemRolesCatalog.Student.Name] = []
        };
}