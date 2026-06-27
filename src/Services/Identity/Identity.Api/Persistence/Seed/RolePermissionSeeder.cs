using Identity.Api.Persistence.Contexts;
using SharedKernel.Persistence.Database;

namespace Identity.Api.Persistence.Seed;

public class RolePermissionSeeder(IdentityDbContext context) : IDataSeeder
{
    public int Order => 2;
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}