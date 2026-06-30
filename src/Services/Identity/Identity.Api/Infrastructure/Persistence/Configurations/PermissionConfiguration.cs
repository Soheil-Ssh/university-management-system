using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Api.Infrastructure.Persistence.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        // Table
        builder.ToTable("Permissions");

        // Primary Key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(id => id.Value,
                value => new PermissionId(value));

        // Name
        builder.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        // Display name
        builder.Property(x => x.DisplayName)
            .HasMaxLength(150)
            .IsRequired();

        // Code
        builder.Property(x => x.Code)
            .HasMaxLength(150)
            .IsRequired();

        builder.HasIndex(x => x.Code)
            .IsUnique();

        // Audit
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
    }
}