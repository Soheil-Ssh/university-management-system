using Academic.Domain.Curriculum.ValueObjects;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academic.Infrastructure.Persistence.Configurations;

public sealed class CurriculumConfiguration : IEntityTypeConfiguration<Curriculum>
{
    public void Configure(EntityTypeBuilder<Curriculum> builder)
    {
        // Table
        builder.ToTable("Curricula", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint("ck_curricula_minimum_required_credits", "\"MinimumRequiredCredits\" > 0");
                tableBuilder.HasCheckConstraint("ck_curricula_effective_dates", "\"EffectiveTo\" IS NULL OR " + "\"EffectiveTo\" >= \"EffectiveFrom\"");
            });

        // Primary key
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, value => new CurriculumId(value))
            .HasColumnName("id")
            .ValueGeneratedNever();

        // MajorId
        builder.Property(x => x.MajorId)
            .HasConversion(id => id.Value, value => new MajorId(value))
            .IsRequired();
        builder.HasIndex(x => x.MajorId).IsUnique().HasFilter("\"Status\" = 2");
        builder.HasOne<Major>()
            .WithMany()
            .HasForeignKey(x => x.MajorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Code
        builder.Property(x => x.Code)
            .HasConversion(code => code.Value, value => RestoreCurriculumCode(value))
            .HasMaxLength(100)
            .IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();

        // Title
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();

        // Version
        builder.Property(x => x.Version).HasMaxLength(50).IsRequired();

        // EffectiveFrom
        builder.Property(x => x.EffectiveFrom).IsRequired();

        // EffectiveTo
        builder.Property(x => x.EffectiveTo);

        // MinimumRequiredCredits
        builder.Property(x => x.MinimumRequiredCredits).IsRequired();

        // Status
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();

        // Description
        builder.Property(x => x.Description).HasMaxLength(1000);

        builder.OwnsMany(x => x.Courses, course =>
        {
            course.ToTable("CurriculumCourses",
                tableBuilder =>
                {
                    tableBuilder.HasCheckConstraint("ck_curriculum_courses_suggested_semester", "\"SuggestedSemester\" > 0");
                    tableBuilder.HasCheckConstraint("ck_curriculum_courses_display_order", "\"DisplayOrder\" IS NULL OR " + "\"DisplayOrder\" > 0");
                });

            course.WithOwner().HasForeignKey(x => x.CurriculumId);

            // Primary key
            course.HasKey(x => x.Id);
            course.Property(x => x.Id)
                .HasConversion(id => id.Value, value => new CurriculumCourseId(value))
                .ValueGeneratedNever();

            // CurriculumId
            course.Property(x => x.CurriculumId)
                .HasConversion(id => id.Value, value => new CurriculumId(value))
                .IsRequired();

            // CourseId
            course.Property(x => x.CourseId)
                .HasConversion(id => id.Value, value => new CourseId(value))
                .IsRequired();
            course.HasIndex(x => x.CourseId);
            course.HasOne<Course>()
                .WithMany()
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Category
            course.Property(x => x.Category).HasConversion<int>().IsRequired();

            // RequirementType
            course.Property(x => x.RequirementType).HasConversion<int>().IsRequired();

            // SuggestedSemester
            course.Property(x => x.SuggestedSemester).IsRequired();

            // DisplayOrder
            course.Property(x => x.DisplayOrder);

            // Indexed
            course.HasIndex(x => new { x.CurriculumId, x.CourseId }).IsUnique();
            course.HasIndex(x => new { x.CurriculumId, x.SuggestedSemester, x.DisplayOrder });

            // Audit
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
        });

        builder.Navigation(x => x.Courses).UsePropertyAccessMode(PropertyAccessMode.Field);

        // Indexed
        builder.HasIndex(x => new { x.MajorId, x.Status });

        // Audit
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
    }

    private static CurriculumCode RestoreCurriculumCode(string value)
    {
        var result = CurriculumCode.FromString(value);

        if (result.IsFailure)
            throw new InvalidOperationException($"Stored CurriculumCode '{value}' is invalid.");

        return result.Data;
    }
}