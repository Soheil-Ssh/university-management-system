using Identity.Api.Domain.Permission;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Persistence.Contexts;

public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
}