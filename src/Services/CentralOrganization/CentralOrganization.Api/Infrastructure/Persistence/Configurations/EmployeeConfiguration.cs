using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Domain.Identifiers;
using SharedKernel.Persistence.Extensions;
using Unit = CentralOrganization.Api.Domain.Unit.Unit;

namespace CentralOrganization.Api.Infrastructure.Persistence.Configurations;

public sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        // Table
        builder.ToTable("Employees");

        // Primary key
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => new EmployeeId(value));

        // UnitId
        builder.Property(x => x.UnitId)
            .HasColumnName("UnitId")
            .IsRequired()
            .HasConversion(id => id.Value, value => new UnitId(value));
        builder.HasIndex(x => x.UnitId);
        builder.HasOne<Unit>()
            .WithMany()
            .HasForeignKey(x => x.UnitId)
            .OnDelete(DeleteBehavior.Restrict);

        // PersonnelCode
        builder.Property(x => x.PersonnelCode)
            .HasColumnName("PersonnelCode")
            .HasMaxLength(17)
            .IsRequired()
            .HasConversion(code => code.Value, value => PersonnelCode.FromString(value).Data);
        builder.HasIndex(x => x.PersonnelCode).IsUnique();

        // FirstName
        builder.ConfigureName(x => x.FirstName, "FirstName", true);

        // LastName
        builder.ConfigureNameAsConversion(x => x.LastName, "LastName", true);

        // FatherName
        builder.Property(x => x.FatherName)
            .HasColumnName("FatherName")
            .HasMaxLength(100)
            .HasConversion(
                name => name == null ? null : name.Value,
                value => string.IsNullOrWhiteSpace(value) ? null : Name.Create(value).Data)
            .IsRequired(false);

        // NationalCode
        builder.ConfigureNationalCodeAsConversion(x => x.NationalCode, "NationalCode", true);
        builder.HasIndex(x => x.NationalCode).IsUnique();

        // BirthDate
        builder.Property(x => x.BirthDate).IsRequired();

        // Gender
        builder.Property(x => x.Gender).HasConversion<int>().IsRequired();

        // MobileNumber
        builder.ConfigureMobile(x => x.MobileNumber, "MobileNumber", true);

        // PhoneNumber
        builder.Property(x => x.PhoneNumber)
            .HasColumnName("PhoneNumber")
            .HasMaxLength(20)
            .HasConversion(
                name => name == null ? null : name.Value,
                value => string.IsNullOrWhiteSpace(value) ? null : PhoneNumber.Create(value).Data)
            .IsRequired(false);

        // Email
        builder.ConfigureEmailAsConversion(x => x.Email, "Email", true);
        builder.HasIndex(x => x.Email).IsUnique();

        // EducationField
        builder.Property(x => x.EducationField).HasMaxLength(150).IsRequired();

        // JobTitle
        builder.Property(x => x.JobTitle).HasMaxLength(150).IsRequired();

        // EmploymentStatus
        builder.Property(x => x.EmploymentStatus).HasConversion<int>().IsRequired();
        builder.HasIndex(x => x.EmploymentStatus);

        // IdentityProvisioningStatus
        builder.Property(x => x.IdentityProvisioningStatus).HasConversion<int>().IsRequired();

        // IdentityUserId
        builder.Property(x => x.IdentityUserId)
            .HasColumnName("IdentityUserId")
            .HasConversion(
                id => id == null ? (Guid?)null : id.Value,
                value => value.HasValue ? new UserId(value.Value) : null);
        builder.HasIndex(x => x.IdentityUserId).IsUnique();

        // ProfileImageFileId
        builder.Property(x => x.ProfileImageFileId)
            .HasColumnName("ProfileImageFileId")
            .HasConversion(
                id => id == null ? (Guid?)null : id.Value,
                value => value.HasValue ? new FileId(value.Value) : null);

        // Search-friendly indexes
        builder.HasIndex(x => x.LastName);
        builder.HasIndex(x => x.JobTitle);

        // Audit
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
    }
}