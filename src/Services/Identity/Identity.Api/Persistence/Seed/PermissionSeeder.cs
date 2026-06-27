using Identity.Api.Common.Authorization.Permissions;
using Identity.Api.Persistence.Contexts;
using SharedKernel.Persistence.Database;

namespace Identity.Api.Persistence.Seed;

public class PermissionSeeder(IdentityDbContext context) : IDataSeeder
{
    public int Order => 0;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await context.Permissions.AnyAsync(cancellationToken)) return;

        foreach (var permission in PermissionRegistry.All)
        {
            var permissionResult = Permission.Create(permission.Name, permission.Code);
            if (permissionResult.IsFailure)
                throw new Exception(permissionResult.Error.ToString());

            await context.Permissions.AddAsync(permissionResult.Data, cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}