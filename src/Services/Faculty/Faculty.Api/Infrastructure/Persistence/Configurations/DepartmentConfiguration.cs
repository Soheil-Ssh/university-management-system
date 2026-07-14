using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faculty.Api.Infrastructure.Persistence.Configurations;

public sealed class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        // Table
        builder.ToTable("Departments");

        // Primary Key
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, value => new DepartmentId(value))
            .ValueGeneratedNever();

        // Faculty id
        builder.Property(x => x.FacultyId)
            .HasConversion(id => id.Value, value => new FacultyId(value))
            .IsRequired();

        builder.HasOne<Domain.Faculty.Faculty>()
            .WithMany()
            .HasForeignKey(x => x.FacultyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Code
        builder.Property(x => x.Code)
            .HasConversion(code => code.Value, value => DepartmentCode.FromString(value).Data)
            .HasMaxLength(50)
            .IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();

        // Name
        builder.Property(x => x.Name).HasMaxLength(150).IsRequired();

        // Short Name
        builder.Property(x => x.ShortName).HasMaxLength(50);

        // Description
        builder.Property(x => x.Description).HasMaxLength(500);

        // Head Professor Id
        builder.Property(x => x.HeadProfessorId)
            .HasConversion(
                id => id == null ? (Guid?)null : id.Value,
                value => value.HasValue
                    ? new ProfessorId(value.Value)
                    : null)
            .IsRequired(false);
        builder.HasIndex(x => x.HeadProfessorId).IsUnique();

        builder.HasOne<Professor>()
            .WithMany()
            .HasForeignKey(x => x.HeadProfessorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Primary Expert Employee Id
        builder.Property(x => x.PrimaryExpertEmployeeId)
            .HasConversion(
                id => id == null ? (Guid?)null : id.Value,
                value => value.HasValue ? new EmployeeId(value.Value) : null)
            .IsRequired(false);
        builder.HasIndex(x => x.PrimaryExpertEmployeeId);

        // Email
        builder.Property(x => x.Email)
            .HasConversion(
                email => email == null ? null : email.Value,
                value => value == null ? null : Email.Create(value).Data)
            .HasMaxLength(254);

        // Phone Number
        builder.Property(x => x.PhoneNumber)
            .HasConversion(
                phoneNumber => phoneNumber == null ? null : phoneNumber.Value,
                value => value == null ? null : PhoneNumber.Create(value).Data)
            .HasMaxLength(30);

        // Internal Phone Number
        builder.Property(x => x.InternalPhoneNumber).HasMaxLength(20);

        // Office Location
        builder.Property(x => x.OfficeLocation).HasMaxLength(200);

        // IsActive
        builder.Property(x => x.IsActive).IsRequired();

        // Audit
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();

        // Indexes
        builder.HasIndex(x => new { x.FacultyId, x.Name }).IsUnique();
        builder.HasIndex(x => new { x.FacultyId, x.IsActive });
    }
}