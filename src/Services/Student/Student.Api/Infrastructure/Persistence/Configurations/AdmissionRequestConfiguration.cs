using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Domain.Identifiers;
using SharedKernel.Persistence.Extensions;
using Student.Api.Domain.Admission;
using Student.Api.Domain.Admission.ValueObjects;
using System.Linq.Expressions;

namespace Student.Api.Infrastructure.Persistence.Configurations;

public class AdmissionRequestConfiguration : IEntityTypeConfiguration<AdmissionRequest>
{
    public void Configure(EntityTypeBuilder<AdmissionRequest> builder)
    {
        // Table
        builder.ToTable("AdmissionRequests");

        // Primary key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => new AdmissionRequestId(value));

        // Registration token
        builder.Property(x => x.RegistrationToken)
            .IsRequired()
            .HasMaxLength(64);

        builder.HasIndex(x => x.RegistrationToken)
            .IsUnique();

        // Status
        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        // Step
        builder.Property(x => x.Step)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.RejectionReason)
            .HasMaxLength(2000);

        builder.Property(x => x.SubmittedAt);

        builder.Property(x => x.ReviewedAt);

        builder.Property(x => x.CreatedAt);

        builder.Property(x => x.UpdatedAt);

        builder.Property(x => x.ReviewerId)
            .HasConversion(
                id => id == null ? (Guid?)null : id.Value,
                value => value.HasValue ? new UserId(value.Value) : null);

        #region TrackingCode

        builder.ComplexProperty(x => x.TrackingCode, tc =>
        {
            tc.Property(x => x.Value)
                .HasColumnName("TrackingCode")
                .HasMaxLength(14)
                .IsRequired();
        });

        builder.HasIndex(x => x.TrackingCode)
            .IsUnique();

        #endregion

        // Attachments as owned collection
        builder.Navigation(x => x.Attachments)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Applicant Personal Info
        ConfigureApplicantPersonalInfo(builder);

        // Applicant Contact Info
        ConfigureApplicantContactInfo(builder);

        // Father
        ConfigureParent(builder, x => x.FatherInfo, "Father");

        // Mother
        ConfigureParent(builder, x => x.MotherInfo, "Mother");

        // Emergency Contact
        ConfigureEmergency(builder);

        // Diploma
        ConfigureDiploma(builder);

        // Entrance
        ConfigureEntrance(builder);

        // Attachments
        ConfigureAttachments(builder);

        // Audit
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
    }

    private static void ConfigureApplicantPersonalInfo(EntityTypeBuilder<AdmissionRequest> builder)
    {
        builder.OwnsOne(x => x.ApplicantPersonalInfo, owned =>
        {
            owned.ConfigureName(x => x.FirstName, "FirstName");
            owned.ConfigureName(x => x.LastName, "LastName");
            owned.ConfigureName(x => x.EnFirstName, "EnFirstName");
            owned.ConfigureName(x => x.EnFirstName, "EnFirstName");
            owned.ConfigureNationalCode(x => x.NationalCode, "NationalCode");

            owned.Property(x => x.BirthPlace)
                .HasMaxLength(100);

            owned.Property(x => x.IssuePlace)
                .HasMaxLength(100);

            owned.Property(x => x.BirthDate);

            owned.Property(x => x.Gender)
                .HasConversion<int>();

            owned.Property(x => x.MaritalStatus)
                .HasConversion<int>();

            owned.OwnsOne(x => x.PersonalImageFileId, file =>
            {
                file.Property(x => x.Value)
                    .HasColumnName("PersonalImageFileId");
            });
        });
    }

    private static void ConfigureApplicantContactInfo(EntityTypeBuilder<AdmissionRequest> builder)
    {
        builder.OwnsOne(x => x.ApplicantContactInfo, owned =>
        {
            owned.ConfigureMobile(x => x.Mobile, "Mobile");

            owned.ConfigurePhone(x => x.Phone, "Phone");

            owned.ConfigureEmail(x => x.Email, "Email");

            owned.ConfigureAddress(x => x.Address);
        });
    }

    private static void ConfigureParent(EntityTypeBuilder<AdmissionRequest> builder,
        Expression<Func<AdmissionRequest, ParentInfo?>> expression,
        string prefix)
    {
        builder.OwnsOne(expression, owned =>
        {
            owned.ConfigureName(x => x.FirstName, $"{prefix}FirstName");

            owned.ConfigureName(x => x.LastName, $"{prefix}LastName");

            owned.ConfigureNationalCode(x => x.NationalCode, $"{prefix}NationalCode");

            owned.ConfigureMobile(x => x.Mobile, $"{prefix}Mobile");
        });
    }

    private static void ConfigureEmergency(EntityTypeBuilder<AdmissionRequest> builder)
    {
        builder.OwnsOne(x => x.EmergencyContact, owned =>
        {
            owned.ConfigureName(x => x.FirstName, "EmergencyContactFirstName");
            owned.ConfigureName(x => x.LastName, "EmergencyContactLastName");
            owned.Property(x => x.Relation).HasMaxLength(100);
            owned.ConfigureMobile(x => x.Mobile, "EmergencyContactMobile");
        });
    }

    private static void ConfigureDiploma(EntityTypeBuilder<AdmissionRequest> builder)
    {
        builder.OwnsOne(x => x.DiplomaInfo, owned =>
        {
            owned.Property(x => x.Average);
            owned.Property(x => x.Field).HasMaxLength(200);
            owned.Property(x => x.GraduationYear);
        });
    }

    private static void ConfigureEntrance(EntityTypeBuilder<AdmissionRequest> builder)
    {
        builder.OwnsOne(x => x.EntranceInfo, owned =>
        {
            owned.Property(x => x.Quota)
                .HasConversion<int>();

            owned.Property(x => x.EntranceExamRank);
            owned.Property(x => x.EntranceScore);

            owned.Property(x => x.AdmissionMethod)
                .HasConversion<int>();

            owned.Property(x => x.AdmissionType)
                .HasConversion<int>();
        });
    }

    private static void ConfigureAttachments(EntityTypeBuilder<AdmissionRequest> builder)
    {
        builder.Navigation(x => x.Attachments)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany(x => x.Attachments, owned =>
        {
            owned.ToTable("AdmissionAttachments");

            owned.WithOwner()
                .HasForeignKey("AdmissionRequestId");

            owned.HasKey(x => x.Id);

            owned.Property(x => x.Id)
                .ValueGeneratedNever()
                .HasConversion(id => id.Value, value => new AdmissionAttachmentId(value));

            owned.Property(x => x.Type)
                .HasConversion<int>();

            owned.OwnsOne(x => x.FileId, file =>
            {
                file.Property(x => x.Value)
                    .HasColumnName("FileId");
            });

            owned.Property(x => x.Description)
                .HasMaxLength(500);

            owned.Property(x => x.CreatedAt);

            owned.Property(x => x.UpdatedAt);
        });
    }
}
