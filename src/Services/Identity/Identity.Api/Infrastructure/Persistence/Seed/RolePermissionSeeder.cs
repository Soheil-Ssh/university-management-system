using Identity.Api.Infrastructure.Authorization;
using Identity.Api.Infrastructure.Persistence.Contexts;
using SharedKernel.Persistence.Database;

namespace Identity.Api.Infrastructure.Persistence.Seed;

public class RolePermissionSeeder(IdentityDbContext context) : IDataSeeder
{
    public int Order => 2;
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var roles = await context.Roles
            .Include(x => x.RolePermissions)
            .ToDictionaryAsync(x => x.Name, cancellationToken);

        var permissions = await context.Permissions
            .ToDictionaryAsync(x => x.Code, cancellationToken);

        foreach (var (roleName, permissionCodes) in RolePermissionRegistry.All)
        {
            if (!roles.TryGetValue(roleName, out var role))
                throw new Exception($"Role '{roleName}' not found.");

            foreach (var permissionCode in permissionCodes)
            {
                if (!permissions.TryGetValue(permissionCode, out var permission))
                    throw new Exception($"Permission '{permissionCode}' not found.");

                if (role.RolePermissions.Any(x => x.PermissionId == permission.Id))
                    continue;

                var result = role.SyncPermissionWithSystemDefinition(permission.Id);
                if (result.IsFailure)
                    throw new Exception(result.Error.ToString());
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}