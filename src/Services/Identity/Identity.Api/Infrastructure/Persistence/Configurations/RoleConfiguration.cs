using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Api.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        // Table
        builder.ToTable("Roles");

        // Primary key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => new RoleId(value));

        // Name
        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .IsUnique();

        // Display Name
        builder.Property(x => x.DisplayName)
            .HasMaxLength(150)
            .IsRequired();

        // Description
        builder.Property(x => x.Description)
            .HasMaxLength(500);

        // Permissions
        builder.Navigation(x => x.RolePermissions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany(x => x.RolePermissions, permission =>
        {
            // Table
            permission.ToTable("RolePermissions");

            // Foreign key
            permission.WithOwner()
                .HasForeignKey("RoleId");

            // Primary key
            permission.HasKey(x => x.Id);

            permission.Property(x => x.Id)
                .ValueGeneratedNever()
                .HasConversion(
                    id => id.Value,
                    value => new RolePermissionId(value));

            // PermissionId
            permission.Property(x => x.PermissionId)
                .HasConversion(
                    id => id.Value,
                    value => new PermissionId(value))
                .IsRequired();

            permission.HasOne<Permission>()
                .WithMany()
                .HasForeignKey(x => x.PermissionId)
                .OnDelete(DeleteBehavior.Restrict);

            permission.HasIndex("RoleId", nameof(RolePermission.PermissionId))
                .IsUnique();

            // Audit
            permission.Property(x => x.CreatedAt).IsRequired();
            permission.Property(x => x.UpdatedAt).IsRequired();
        });

        // Audit
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
    }
}