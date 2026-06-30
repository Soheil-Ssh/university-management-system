using Identity.Api.Common.Authorization.Permissions;
using SharedKernel.Persistence.Database;

namespace Identity.Api.Persistence.Seed;

public class PermissionSeeder(IdentityDbContext context) : IDataSeeder
{
    public int Order => 0;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var permissions = await context.Permissions.ToDictionaryAsync(x => x.Code, cancellationToken);

        foreach (var definition in PermissionRegistry.All)
        {
            if (!permissions.TryGetValue(definition.Code, out var permission))
            {
                var permissionResult = Permission.Create(definition.Name,
                    definition.DisplayName,
                    definition.Code);

                if (permissionResult.IsFailure)
                    throw new Exception(permissionResult.Error.ToString());

                context.Permissions.Add(permissionResult.Data);

                permissions.Add(permissionResult.Data.Code, permissionResult.Data);
            }
            else
            {
                var syncResult = permission.SyncWithSystemDefinition(definition.Name,
                    definition.DisplayName);

                if (syncResult.IsFailure)
                    throw new Exception(syncResult.Error.ToString());
            }
        }


        await context.SaveChangesAsync(cancellationToken);
    }
}