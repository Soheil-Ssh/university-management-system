using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faculty.Api.Infrastructure.Persistence.Configurations;

public class DepartmentProfessorAssignmentConfiguration : IEntityTypeConfiguration<DepartmentProfessorAssignment>
{
    public void Configure(EntityTypeBuilder<DepartmentProfessorAssignment> builder)
    {
        // Table
        builder.ToTable("DepartmentProfessorAssignments");

        // Primary Key
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, value => new DepartmentProfessorAssignmentId(value))
            .ValueGeneratedNever();

        // DepartmentId
        builder.Property(x => x.DepartmentId)
            .HasConversion(id => id.Value, value => new DepartmentId(value))
            .IsRequired();
        builder.HasIndex(x => x.DepartmentId).IsUnique();

        // ProfessorId
        builder.Property(x => x.ProfessorId)
            .HasConversion(id => id.Value, value => new ProfessorId(value))
            .IsRequired();
        builder.HasIndex(x => x.ProfessorId).IsUnique();

        // AssignedAt
        builder.Property(x => x.AssignedAt).IsRequired();

        // UnassignedAt
        builder.Property(x => x.UnassignedAt).IsRequired(false);

        // Is Active
        builder.Ignore(x => x.IsActive);

        // Department relations
        builder.HasOne<Department>()
            .WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Professor relations
        builder.HasOne<Professor>()
            .WithMany()
            .HasForeignKey(x => x.ProfessorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index
        builder.HasIndex(x => new { x.DepartmentId, x.ProfessorId })
            .IsUnique()
            .HasFilter("\"UnassignedAt\" IS NULL")
            .HasDatabaseName("UX_DepartmentProfessorAssignments_ActiveAssignment");

        // Audit
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
    }
}