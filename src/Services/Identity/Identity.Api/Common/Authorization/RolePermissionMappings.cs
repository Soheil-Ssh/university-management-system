using Identity.Api.Common.Authorization.Permissions;

namespace Identity.Api.Common.Authorization;

public class RolePermissionMappings
{
    public static readonly Dictionary<string, IReadOnlyCollection<string>> Mappings =
        new()
        {
            [Roles.Roles.SuperAdmin] = [..PermissionRegistry.All.Select(x => x.Name)],

            [Roles.Roles.UniversityPresident] = [..PermissionRegistry.All.Select(x => x.Name)],

            [Roles.Roles.Professor] = [],

            [Roles.Roles.Student] = []
        };
}