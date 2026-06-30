using Identity.Api.Domain.Permission;
using Identity.Api.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Infrastructure.Persistence.Repositories;

public class PermissionRepository(IdentityDbContext context) : IPermissionRepository
{
}