using Identity.Api.Common.Authorization.Permissions;

namespace Identity.Api.Common.Authorization;

public class RolePermissionRegistry
{
    public static readonly Dictionary<string, IReadOnlyCollection<string>> All =
        new()
        {
            [Roles.RolesRegistry.SuperAdmin.Name] = [..PermissionRegistry.All.Select(x => x.Code)],

            [Roles.RolesRegistry.UniversityPresident.Name] = [..PermissionRegistry.All.Select(x => x.Code)],

            [Roles.RolesRegistry.Professor.Name] = [],

            [Roles.RolesRegistry.Student.Name] = []
        };
}