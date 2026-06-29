using Identity.Api.Common.Authorization.Permissions;
using SharedKernel.Persistence.Database;

namespace Identity.Api.Persistence.Seed;

public class PermissionSeeder(IdentityDbContext context) : IDataSeeder
{
    public int Order => 0;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        foreach (var definition in PermissionRegistry.All)
        {
            var permission = await context.Permissions
                .FirstOrDefaultAsync(x => x.Code == definition.Code, cancellationToken);

            if (permission is null)
            {
                var permissionResult = Permission.Create(definition.Name, definition.DisplayName, definition.Code);
                if (permissionResult.IsFailure)
                    throw new Exception(permissionResult.Error.ToString());

                await context.Permissions.AddAsync(permissionResult.Data, cancellationToken);
            }
            else
            {
                var syncResult = permission.SyncWithSystemDefinition(definition.Name, definition.DisplayName);
                if (syncResult.IsFailure)
                    throw new Exception(syncResult.Error.ToString());
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}