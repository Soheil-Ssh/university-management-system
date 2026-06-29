using Identity.Api.Common.Authorization.Permissions;

namespace Identity.Api.Common.Authorization;

public class RolePermissionMappings
{
    public static readonly Dictionary<string, IReadOnlyCollection<string>> Mappings =
        new()
        {
            [Roles.Roles.SuperAdmin.Name] = [..PermissionRegistry.All.Select(x => x.Name)],

            [Roles.Roles.UniversityPresident.Name] = [..PermissionRegistry.All.Select(x => x.Name)],

            [Roles.Roles.Professor.Name] = [],

            [Roles.Roles.Student.Name] = []
        };
}