using System.Reflection;
using SharedKernel.Messaging.MassTransit.Extensions;
using Unit = CentralOrganization.Api.Domain.Unit.Unit;

namespace CentralOrganization.Api.Infrastructure.Persistence.Contexts;

public class CentralOrganizationDbContext(DbContextOptions<CentralOrganizationDbContext> options)
    : DbContext(options)
{
    public DbSet<Unit> Units { get; set; }
    public DbSet<Employee> Employees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.AddMassTransitOutboxEntities();
        base.OnModelCreating(modelBuilder);
    }
}