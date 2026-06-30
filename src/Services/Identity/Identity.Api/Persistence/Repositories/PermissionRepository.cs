using Identity.Api.Domain.Permission;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Persistence.Repositories;

public class PermissionRepository(IdentityDbContext context) : IPermissionRepository
{
}