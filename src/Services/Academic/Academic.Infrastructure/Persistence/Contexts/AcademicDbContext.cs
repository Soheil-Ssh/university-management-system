using System.Reflection;

namespace Academic.Infrastructure.Persistence.Contexts;

public sealed class AcademicDbContext(DbContextOptions<AcademicDbContext> options) : DbContext(options)
{
    public DbSet<Major> Majors { get; set; }
    public DbSet<Curriculum> Curricula { get; set; }
    public DbSet<Course> Courses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}