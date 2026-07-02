using File.Api.Domain.File.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Domain.Identifiers;

namespace File.Api.Infrastructure.Persistence.Configurations;

public class FileConfiguration : IEntityTypeConfiguration<Domain.File.File>
{
    public void Configure(EntityTypeBuilder<Domain.File.File> builder)
    {
        // Table
        builder.ToTable("Files");

        // Primary key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => new FileId(value));

        // FileName
        builder.ComplexProperty(x => x.FileName, fileName =>
        {
            fileName.Property(x => x.Value)
                .HasColumnName("FileName")
                .IsRequired()
                .HasMaxLength(255);
        });

        // MimeType
        builder.Property(x => x.MimeType)
            .IsRequired()
            .HasMaxLength(100);

        // Hash
        builder.OwnsOne(x => x.Hash, hash =>
        {
            hash.Property(x => x.Value)
                .HasColumnName("Hash")
                .HasMaxLength(64);

            hash.HasIndex(x => x.Value)
                .IsUnique();
        });

        // Size
        builder.Property(x => x.Size)
            .IsRequired();

        // Status
        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion(
                status => status.ToString(),
                value => Enum.Parse<FileStatus>(value))
            .HasMaxLength(20);

        builder.HasIndex(x => x.Status);

        // UploadedBy
        builder.Property(x => x.UploadedBy)
            .HasConversion(
                userId => userId != null ? userId.Value : (Guid?)null,
                value => value.HasValue ? new UserId(value.Value) : null);
        builder.HasIndex(x => x.UploadedBy);

        // AttachedAt
        builder.Property(x => x.AttachedAt);
        builder.HasIndex(x => x.AttachedAt);

        // DeletedAt
        builder.Property(x => x.DeletedAt);

        // Audit
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
    }
}