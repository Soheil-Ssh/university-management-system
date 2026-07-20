using Academic.Domain.Major.ValueObjects;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academic.Infrastructure.Persistence.Configurations;

public sealed class MajorConfiguration: IEntityTypeConfiguration<Major>
{
    public void Configure(EntityTypeBuilder<Major> builder)
    {
        // Table
        builder.ToTable("Majors");

        // Primary key
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, value => new MajorId(value))
            .ValueGeneratedNever();

        // DepartmentId
        builder.Property(x => x.DepartmentId)
            .HasConversion(id => id.Value, value => new DepartmentId(value))
            .IsRequired();
        builder.HasIndex(x => x.DepartmentId).IsUnique();

        // Code
        builder.Property(x => x.Code)
            .HasConversion(code => code.Value, value => RestoreMajorCode(value))
            .HasMaxLength(50)
            .IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();

        // Name
        builder.Property(x => x.Name).HasMaxLength(150).IsRequired();

        // Description
        builder.Property(x => x.Description).HasMaxLength(500);

        // IsActive
        builder.Property(x => x.IsActive).IsRequired();

        // Audit
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();

        // Indexes
        builder.HasIndex(x => new { x.DepartmentId, x.IsActive });
    }

    private static MajorCode RestoreMajorCode(string value)
    {
        var result = MajorCode.FromString(value);

        if (result.IsFailure)
            throw new InvalidOperationException(
                $"Stored MajorCode '{value}' is invalid.");

        return result.Data;
    }
}