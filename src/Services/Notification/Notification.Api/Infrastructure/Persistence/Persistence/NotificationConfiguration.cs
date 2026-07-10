using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Notification.Api.Infrastructure.Persistence.Persistence;

public sealed class NotificationMessageConfiguration : IEntityTypeConfiguration<NotificationMessage>
{
    public void Configure(EntityTypeBuilder<NotificationMessage> builder)
    {
        // Table
        builder.ToTable("Notifications");

        // Primary Key
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, value => new NotificationMessageId(value))
            .ValueGeneratedNever();

        // CorrelationId
        builder.Property(x => x.CorrelationId).HasMaxLength(100);

        // SourceService
        builder.Property(x => x.SourceService).HasMaxLength(150);

        // SourceEventId
        builder.Property(x => x.SourceEventId);

        // SourceEventType
        builder.Property(x => x.SourceEventType).HasMaxLength(250);

        // RecipientUserId
        builder.Property(x => x.RecipientUserId);

        // RecipientEmail
        builder.Property(x => x.RecipientEmail).HasMaxLength(320);

        // RecipientMobile
        builder.Property(x => x.RecipientMobile).HasMaxLength(30);

        // RecipientDeviceToken
        builder.Property(x => x.RecipientDeviceToken).HasMaxLength(2048);

        // Channel
        builder.Property(x => x.Channel).HasConversion<int>().IsRequired();

        // Subject
        builder.Property(x => x.Subject).HasMaxLength(200);

        // Body
        builder.Property(x => x.Body).HasMaxLength(4000).IsRequired();

        // Status
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();

        // Priority
        builder.Property(x => x.Priority).HasConversion<int>().IsRequired();


        // ScheduledAt
        builder.Property(x => x.ScheduledAt);

        // SentAt
        builder.Property(x => x.SentAt);

        // FailedAt
        builder.Property(x => x.FailedAt);

        // FailureReason
        builder.Property(x => x.FailureReason).HasMaxLength(1000);

        // DeliveryAttemptCount
        builder.Property(x => x.DeliveryAttemptCount).IsRequired();

        // DeliveryAttempts
        builder.OwnsMany(x => x.DeliveryAttempts, attempt =>
        {
            // Table
            attempt.ToTable("NotificationDeliveryAttempts");

            // Foreign Key
            attempt.WithOwner()
                .HasForeignKey(x => x.NotificationMessageId);

            // Primary Key
            attempt.HasKey(x => x.Id);
            attempt.Property(x => x.Id)
                .HasConversion(id => id.Value, value => new NotificationDeliveryAttemptId(value))
                .ValueGeneratedNever();


            // NotificationMessageId
            attempt.Property(x => x.NotificationMessageId)
                .HasConversion(id => id.Value, value => new NotificationMessageId(value))
                .IsRequired();

            // Channel
            attempt.Property(x => x.Channel).HasConversion<int>().IsRequired();

            // Provider
            attempt.Property(x => x.Provider).HasConversion<int>().IsRequired();

            // AttemptNumber
            attempt.Property(x => x.AttemptNumber).IsRequired();

            // Status
            attempt.Property(x => x.Status).HasConversion<int>().IsRequired();

            // ProviderMessageId
            attempt.Property(x => x.ProviderMessageId).HasMaxLength(200);

            // ErrorCode
            attempt.Property(x => x.ErrorCode).HasMaxLength(100);

            // ErrorMessage
            attempt.Property(x => x.ErrorMessage).HasMaxLength(1000);

            // CompletedAt
            attempt.Property(x => x.CompletedAt);

            // Index
            attempt.HasIndex(x => new { x.NotificationMessageId, x.AttemptNumber }).IsUnique();

            // Audit
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
        });

        builder.Navigation(x => x.DeliveryAttempts)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // SourceEventId
        builder.HasIndex(x => x.SourceEventId);

        // Indexes
        builder.HasIndex(x => new { x.SourceService, x.SourceEventType });
        builder.HasIndex(x => new { x.Status, ScheduledAtUtc = x.ScheduledAt });
        builder.HasIndex(x => new { x.Channel, x.Status });

        // Audit
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
    }
}