using Identity.Api.Common.Authorization.Roles;
using SharedKernel.Persistence.Database;

namespace Identity.Api.Persistence.Seed;

public class RoleSeeder(IdentityDbContext context) : IDataSeeder
{
    public int Order => 1;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        foreach (var definition in Roles.All)
        {
            var role = await context.Roles
                .FirstOrDefaultAsync(x => x.Name == definition.Name, cancellationToken);

            if (role is null)
            {
                var roleResult = Role.CreateSystemRole(definition.Name, definition.DisplayName, definition.Description);
                if (roleResult.IsFailure)
                    throw new Exception(roleResult.Error.ToString());

                await context.Roles.AddAsync(roleResult.Data, cancellationToken);
            }
            else
            {
                var syncResult = role.SyncWithSystemDefinition(definition.DisplayName, definition.Description);
                if (syncResult.IsFailure)
                    throw new Exception(syncResult.Error.ToString());
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}