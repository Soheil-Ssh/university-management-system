namespace Identity.Api.Domain.Role;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(RoleId id, CancellationToken cancellationToken = default);
    Task AddAsync(Role role, CancellationToken cancellationToken = default);
    Task IsExistRole(string roleName, CancellationToken cancellationToken = default);
    Task IsExistRole(RoleId id, string roleName, CancellationToken cancellationToken = default);
}