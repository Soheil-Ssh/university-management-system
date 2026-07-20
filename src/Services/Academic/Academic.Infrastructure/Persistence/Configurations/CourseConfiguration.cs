using Academic.Domain.Course.ValueObjects;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academic.Infrastructure.Persistence.Configurations;

public sealed class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        // Table
        builder.ToTable("Courses", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint("ck_courses_theoretical_credits", "\"TheoreticalCredits\" >= 0");
                tableBuilder.HasCheckConstraint("ck_courses_practical_credits", "\"PracticalCredits\" >= 0");
                tableBuilder.HasCheckConstraint("ck_courses_total_credits", "\"TheoreticalCredits\" + \"PracticalCredits\" > 0");
            });

        // Primary key
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, value => new CourseId(value))
            .ValueGeneratedNever();

        // DepartmentId
        builder.Property(x => x.DepartmentId)
            .HasConversion(id => id.Value, value => new DepartmentId(value))
            .IsRequired();
        builder.HasIndex(x => x.DepartmentId);

        // Code
        builder.Property(x => x.Code)
            .HasConversion(code => code.Value, value => RestoreCourseCode(value))
            .HasMaxLength(50)
            .IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();

        // Title
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();

        // TheoreticalCredits
        builder.Property(x => x.TheoreticalCredits).IsRequired();

        // PracticalCredits
        builder.Property(x => x.PracticalCredits).IsRequired();

        // TotalCredits
        builder.Ignore(x => x.TotalCredits);

        // Description
        builder.Property(x => x.Description).HasMaxLength(1000);

        // IsActive
        builder.Property(x => x.IsActive).IsRequired();

        builder.OwnsMany(x => x.Prerequisites, prerequisites =>
        {
            prerequisites.ToTable("CoursePrerequisites", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint("ck_course_prerequisites_not_self", "\"CourseId\" <> \"PrerequisiteCourseId\"");
            });

            // Relation
            prerequisites.WithOwner().HasForeignKey(x => x.CourseId);
            prerequisites.HasOne<Course>()
                .WithMany()
                .HasForeignKey(x => x.PrerequisiteCourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Primary key
            prerequisites.HasKey(x => x.Id);
            prerequisites.Property(x => x.Id)
                .HasConversion(id => id.Value, value => new CoursePrerequisiteId(value))
                .ValueGeneratedNever();

            // CourseId
            prerequisites.Property(x => x.CourseId)
                .HasConversion(id => id.Value, value => new CourseId(value))
                .IsRequired();

            // PrerequisiteCourseId
            prerequisites.Property(x => x.PrerequisiteCourseId)
                .HasConversion(id => id.Value, value => new CourseId(value))
                .IsRequired();
            prerequisites.HasIndex(x => x.PrerequisiteCourseId);

            // Indexed
            prerequisites.HasIndex(x => new { x.CourseId, x.PrerequisiteCourseId }).IsUnique();

            // Audit
            prerequisites.Property(x => x.CreatedAt).IsRequired();
            prerequisites.Property(x => x.UpdatedAt).IsRequired();
        });

       // Indexes
       builder.HasIndex(x => new { x.DepartmentId, x.IsActive });

        // Audit
       builder.Property(x => x.CreatedAt).IsRequired();
       builder.Property(x => x.UpdatedAt).IsRequired();
    }

    private static CourseCode RestoreCourseCode(string value)
    {
        var result = CourseCode.FromString(value);

        if (result.IsFailure)
            throw new InvalidOperationException(
                $"Stored CourseCode '{value}' is invalid.");

        return result.Data;
    }
}