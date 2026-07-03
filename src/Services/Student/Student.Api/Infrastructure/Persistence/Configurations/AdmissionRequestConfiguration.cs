using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Domain.Identifiers;
using SharedKernel.Persistence.Extensions;
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

        // Tracking code
        builder.OwnsOne(x => x.TrackingCode, trackingCode =>
        {
            trackingCode.Property(x => x.Value)
                .HasColumnName("TrackingCode")
                .HasMaxLength(14)
                .IsRequired();

            trackingCode.HasIndex(x => x.Value)
                .IsUnique();
        });

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
        builder.ComplexProperty(x => x.ApplicantPersonalInfo, property =>
        {
            property.IsRequired(false);
            property.HasDiscriminator<bool>("HasApplicantPersonalInfo").HasValue(true);

            property.ConfigureName(x => x.FirstName, "FirstName");
            property.ConfigureName(x => x.LastName, "LastName");
            property.ConfigureName(x => x.EnFirstName, "EnFirstName");
            property.ConfigureName(x => x.EnLastName, "EnLastName");
            property.ConfigureNationalCode(x => x.NationalCode, "NationalCode");

            property.Property(x => x.BirthPlace)
                .HasMaxLength(100);

            property.Property(x => x.IssuePlace)
                .HasMaxLength(100);

            property.Property(x => x.BirthDate);

            property.Property(x => x.Gender)
                .HasConversion<int>();

            property.Property(x => x.MaritalStatus)
                .HasConversion<int>();

            property.ComplexProperty(x => x.PersonalImageFileId, file =>
            {
                file.Property(x => x.Value)
                    .HasColumnName("PersonalImageFileId");
            });
        });
    }

    private static void ConfigureApplicantContactInfo(EntityTypeBuilder<AdmissionRequest> builder)
    {
        builder.ComplexProperty(x => x.ApplicantContactInfo, property =>
        {
            property.IsRequired(false);
            property.HasDiscriminator<bool>("HasApplicantContactInfo").HasValue(true);

            property.ConfigureMobile(x => x.Mobile, "Mobile");
            property.ConfigurePhone(x => x.Phone, "Phone");
            property.ConfigureEmail(x => x.Email, "Email");
            property.ConfigureAddress(x => x.Address);
        });
    }

    private static void ConfigureParent(EntityTypeBuilder<AdmissionRequest> builder,
        Expression<Func<AdmissionRequest, ParentInfo?>> expression,
        string prefix)
    {
        builder.ComplexProperty(expression, property =>
        {
            property.IsRequired(false);
            property.HasDiscriminator<bool>($"Has{prefix}Info").HasValue(true);

            property.ConfigureName(x => x.FirstName, $"{prefix}FirstName");
            property.ConfigureName(x => x.LastName, $"{prefix}LastName");
            property.ConfigureNationalCode(x => x.NationalCode, $"{prefix}NationalCode");
            property.ConfigureMobile(x => x.Mobile, $"{prefix}Mobile");
        });
    }

    private static void ConfigureEmergency(EntityTypeBuilder<AdmissionRequest> builder)
    {
        builder.ComplexProperty(x => x.EmergencyContact, property =>
        {
            property.IsRequired(false);
            property.HasDiscriminator<bool>("HasEmergencyContact").HasValue(true);

            property.ConfigureName(x => x.FirstName, "EmergencyContactFirstName");
            property.ConfigureName(x => x.LastName, "EmergencyContactLastName");
            property.Property(x => x.Relation).HasMaxLength(100);
            property.ConfigureMobile(x => x.Mobile, "EmergencyContactMobile");
        });
    }

    private static void ConfigureDiploma(EntityTypeBuilder<AdmissionRequest> builder)
    {
        builder.ComplexProperty(x => x.DiplomaInfo, property =>
        {
            property.IsRequired(false);
            property.HasDiscriminator<bool>("HasDiplomaInfo").HasValue(true);

            property.Property(x => x.Average);
            property.Property(x => x.Field).HasMaxLength(200);
            property.Property(x => x.GraduationYear);
        });
    }

    private static void ConfigureEntrance(EntityTypeBuilder<AdmissionRequest> builder)
    {
        builder.ComplexProperty(x => x.EntranceInfo, property =>
        {
            property.IsRequired(false);
            property.HasDiscriminator<bool>("HasEntranceInfo").HasValue(true);

            property.Property(x => x.Quota)
                .HasConversion<int>();

            property.Property(x => x.EntranceExamRank);
            property.Property(x => x.EntranceScore);

            property.Property(x => x.AdmissionMethod)
                .HasConversion<int>();

            property.Property(x => x.AdmissionType)
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
