using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Api.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Table
        builder.ToTable("Users");

        // Primary key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => new UserId(value));

        // UserName
        builder.Property(x => x.UserName)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.UserName)
            .IsUnique();

        // Password
        builder.Property(x => x.PasswordHash)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        // Email
        builder.OwnsOne(x => x.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .HasMaxLength(256)
                .IsRequired();

            email.HasIndex(e => e.Value)
                .IsUnique();
        });

        // UserRoles
        builder.Navigation(x => x.UserRoles)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany(x => x.UserRoles, role =>
        {
            role.ToTable("UserRoles");

            role.WithOwner()
                .HasForeignKey("UserId");

            role.HasKey(x => x.Id);

            role.Property(x => x.Id)
                .ValueGeneratedNever()
                .HasConversion(
                    id => id.Value,
                    value => new UserRoleId(value));

            role.HasOne<Role>()
                .WithMany()
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            role.Property(x => x.RoleId)
                .HasConversion(
                    id => id.Value,
                    value => new RoleId(value))
                .IsRequired();

            role.HasIndex("UserId", nameof(UserRole.RoleId))
                .IsUnique();

            role.Property(x => x.CreatedAt)
                .IsRequired();

            role.Property(x => x.UpdatedAt)
                .IsRequired();
        });

        // Audit
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
    }
}