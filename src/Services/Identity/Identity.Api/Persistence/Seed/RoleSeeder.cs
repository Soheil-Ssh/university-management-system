using Identity.Api.Common.Authorization.Roles;
using SharedKernel.Persistence.Database;

namespace Identity.Api.Persistence.Seed;

public class RoleSeeder(IdentityDbContext context) : IDataSeeder
{
    public int Order => 1;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await context.Roles.AnyAsync(cancellationToken)) return;

        foreach (var role in Roles.All)
        {
            var roleResult = Role.Create(role);
            if (roleResult.IsFailure)
                throw new Exception(roleResult.Error.ToString());

            await context.Roles.AddAsync(roleResult.Data, cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}