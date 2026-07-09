using System.Reflection;
using SharedKernel.Messaging.MassTransit.Extensions;

namespace Identity.Api.Infrastructure.Persistence.Contexts;

public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.AddMassTransitOutboxEntities();
        base.OnModelCreating(modelBuilder);
    }
}