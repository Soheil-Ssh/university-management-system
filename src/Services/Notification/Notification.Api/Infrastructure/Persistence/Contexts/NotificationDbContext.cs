using System.Reflection;
using SharedKernel.Messaging.MassTransit.Extensions;

namespace Notification.Api.Infrastructure.Persistence.Contexts;

public class NotificationDbContext(DbContextOptions<NotificationDbContext> options) : DbContext(options)
{
    public DbSet<NotificationMessage> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.AddMassTransitOutboxEntities();
        base.OnModelCreating(modelBuilder);
    }
}