using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace File.Api.Infrastructure.Persistence.Contexts;

public class FileContext(DbContextOptions<FileContext> options) : DbContext(options)
{
    public DbSet<Domain.File.File> Files { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}