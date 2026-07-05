using Identity.Api.Infrastructure.Authorization.Roles;
using SharedKernel.Persistence.Database;

namespace Identity.Api.Infrastructure.Persistence.Seed;

public class RoleSeeder(IdentityDbContext context) : IDataSeeder
{
    public int Order => 1;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var roles = await context.Roles.ToDictionaryAsync(x => x.Name, cancellationToken);

        foreach (var definition in SystemRolesCatalog.All)
        {
            if (!roles.TryGetValue(definition.Name, out var role))
            {
                var roleResult = Role.CreateSystemRole(
                    definition.Name,
                    definition.DisplayName,
                    definition.Description);

                if (roleResult.IsFailure)
                    throw new Exception(roleResult.Error.ToString());

                context.Roles.Add(roleResult.Data);

                roles.Add(roleResult.Data.Name, roleResult.Data);
            }
            else
            {
                var syncResult = role.SyncWithSystemDefinition(definition.DisplayName,
                    definition.Description);

                if (syncResult.IsFailure)
                    throw new Exception(syncResult.Error.ToString());
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}