using System.Reflection;

namespace Faculty.Api.Infrastructure.Persistence.Contexts;

public sealed class FacultyDbContext(DbContextOptions<FacultyDbContext> options) : DbContext(options)
{
    public DbSet<Domain.Faculty.Faculty> Faculties { get; set; }
    public DbSet<Professor> Professors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}