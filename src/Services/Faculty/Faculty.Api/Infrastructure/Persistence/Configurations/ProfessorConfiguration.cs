using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Persistence.Extensions;

namespace Faculty.Api.Infrastructure.Persistence.Configurations;

public sealed class ProfessorConfiguration : IEntityTypeConfiguration<Professor>
{
    public void Configure(EntityTypeBuilder<Professor> builder)
    {
        // Table
        builder.ToTable("Professors");

        // Primary key
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => new ProfessorId(value));

        // Code
        builder.Property(x => x.Code)
            .HasConversion(code => code.Value, value => ProfessorCode.FromPersistence(value))
            .HasMaxLength(19)
            .IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();

        // FirstName
        builder.ConfigureName(x => x.FirstName, "FirstName", true);

        // LastName
        builder.ConfigureName(x => x.LastName, "LastName", true);

        // FatherName
        builder.ConfigureName(x => x.FatherName, "FatherName", true);

        // NationalCode
        builder.ConfigureNationalCodeAsConversion(x => x.NationalCode, "NationalCode", true);
        builder.HasIndex(x => x.NationalCode).IsUnique();

        // Email
        builder.ConfigureEmailAsConversion(x => x.Email, "Email", true);
        builder.HasIndex(x => x.Email).IsUnique();

        // MobileNumber
        builder.ConfigureMobileAsConversion(x => x.MobileNumber, "MobileNumber", true);
        builder.HasIndex(x => x.MobileNumber).IsUnique();

        // Specialization
        builder.Property(x => x.Specialization).HasMaxLength(150).IsRequired();

        // AcademicRank
        builder.Property(x => x.AcademicRank).HasConversion<int>().IsRequired();

        // EmploymentType
        builder.Property(x => x.EmploymentType).HasConversion<int>().IsRequired();

        // EmploymentStartDate
        builder.Property(x => x.EmploymentStartDate).HasColumnType("date").IsRequired();

        // ProfileImageFileId
        builder.Property(x => x.ProfileImageFileId)
            .HasConversion(
                id => id == null ? (Guid?)null : id.Value,
                value => value == null ? null : new FileId(value.Value))
            .IsRequired(false);

        // IdentityProvisioningStatus
        builder.Property(x => x.IdentityProvisioningStatus).HasConversion<int>().IsRequired();

        // IdentityProvisioningFailureReason
        builder.Property(x => x.IdentityProvisioningFailureReason).HasMaxLength(1000).IsRequired(false);

        // IdentityUserId
        builder.Property(x => x.IdentityUserId)
            .HasConversion(
                id => id == null ? (Guid?)null : id.Value,
                value => value == null ? null : new UserId(value.Value))
            .IsRequired(false);
        builder.HasIndex(x => x.IdentityUserId).IsUnique();

        // IsActive
        builder.Property(x => x.IsActive).IsRequired();

        // Computed property
        builder.Ignore(x => x.FullName);

        // Audit
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
    }
}