using Faculty.Api.Infrastructure.Messaging.Sagas.ProfessorIdentityProvisioning.States;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faculty.Api.Infrastructure.Persistence.Configurations;

public sealed class ProfessorIdentityProvisioningStateConfiguration : IEntityTypeConfiguration<ProfessorIdentityProvisioningState>
{
    public void Configure(EntityTypeBuilder<ProfessorIdentityProvisioningState> builder)
    {
        builder.ToTable("ProfessorIdentityProvisioningSagas");
        builder.HasKey(x => x.CorrelationId);
        builder.Property(x => x.CurrentState).HasMaxLength(100).IsRequired();
        builder.Property(x => x.ProfessorId).IsRequired();
        builder.Property(x => x.NationalCode).HasMaxLength(10).IsRequired();
        builder.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.LastName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(256).IsRequired();
        builder.Property(x => x.MobileNumber).HasMaxLength(20).IsRequired();
        builder.Property(x => x.FailureReason).HasMaxLength(1000);
        builder.HasIndex(x => x.ProfessorId).IsUnique();
    }
}