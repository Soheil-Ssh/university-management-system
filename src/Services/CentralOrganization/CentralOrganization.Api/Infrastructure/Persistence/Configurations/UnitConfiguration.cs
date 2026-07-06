using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unit = CentralOrganization.Api.Domain.Unit.Unit;

namespace CentralOrganization.Api.Infrastructure.Persistence.Configurations;

public sealed class UnitConfiguration : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        // Table
        builder.ToTable("Units");

        // Primary key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever()
            .HasConversion(id => id.Value, value => new UnitId(value));

        // Name
        builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
        builder.HasIndex(x => x.Name).IsUnique();

        // Code
        builder.Property(x => x.Code)
            .HasColumnName("Code")
            .HasMaxLength(11)
            .IsRequired()
            .HasConversion(code => code.Value, value => UnitCode.FromString(value).Data);
        builder.HasIndex(x => x.Code).IsUnique();

        // Description
        builder.Property(x => x.Description)
            .HasMaxLength(500);

        // IsActive
        builder.Property(x => x.IsActive).IsRequired();
        builder.HasIndex(x => x.IsActive);

        // Audit
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
    }
}