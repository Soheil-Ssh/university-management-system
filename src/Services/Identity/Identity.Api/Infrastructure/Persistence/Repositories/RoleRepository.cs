using Identity.Api.Infrastructure.Persistence.Contexts;

namespace Identity.Api.Infrastructure.Persistence.Repositories;

public class RoleRepository(IdentityDbContext context) : IRoleRepository
{
    public async Task<Role?> GetByIdAsync(RoleId id, CancellationToken cancellationToken = default)
        => await context.Roles.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task AddAsync(Role role, CancellationToken cancellationToken = default)
    {
        await context.Roles.AddAsync(role, cancellationToken);
    }

    public async Task<bool> IsExistRole(string roleName, CancellationToken cancellationToken = default)
        => await context.Roles.AnyAsync(r => r.Name.ToLower() == roleName.ToLower(), cancellationToken);

    public async Task<bool> IsExistRole(RoleId id, string roleName, CancellationToken cancellationToken = default)
        => await context.Roles.AnyAsync(r => r.Id != id && r.Name.ToLower() == roleName.ToLower(), cancellationToken);
}