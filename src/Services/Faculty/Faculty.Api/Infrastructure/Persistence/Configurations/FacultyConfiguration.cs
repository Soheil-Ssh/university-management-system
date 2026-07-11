using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faculty.Api.Infrastructure.Persistence.Configurations;

public sealed class FacultyConfiguration : IEntityTypeConfiguration<Domain.Faculty.Faculty>
{
    public void Configure(EntityTypeBuilder<Domain.Faculty.Faculty> builder)
    {
        // Table
        builder.ToTable("Faculties");

        // Primary key
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => new FacultyId(value));

        // Code
        builder.Property(x => x.Code)
            .HasConversion(code => code.Value, value => FacultyCode.FromPersistence(value))
            .HasMaxLength(20)
            .IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();

        // Name
        builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
        builder.HasIndex(x => x.Name).IsUnique();

        // Description
        builder.Property(x => x.Description).HasMaxLength(500);

        // IsActive
        builder.Property(x => x.IsActive).IsRequired();

        // Audit
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
    }
}