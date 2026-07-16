using Faculty.Api.Infrastructure.Messaging.Sagas.ProfessorIdentityDeactivation.States;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faculty.Api.Infrastructure.Persistence.Configurations;

public class ProfessorIdentityDeactivationStateConfiguration : IEntityTypeConfiguration<ProfessorIdentityDeactivationState>
{
    public void Configure(EntityTypeBuilder<ProfessorIdentityDeactivationState> builder)
    {
        builder.ToTable("ProfessorIdentityDeactivationStates");
        builder.HasKey(x => x.CorrelationId);
        builder.Property(x => x.CurrentState).HasMaxLength(100).IsRequired();
        builder.Property(x => x.FailureReason).HasMaxLength(1000);
        builder.HasIndex(x => x.ProfessorId);
        builder.HasIndex(x => x.IdentityUserId);
    }
}