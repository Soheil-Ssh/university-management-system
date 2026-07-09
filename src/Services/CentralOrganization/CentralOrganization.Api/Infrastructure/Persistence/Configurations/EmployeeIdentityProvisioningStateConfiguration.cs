using CentralOrganization.Api.Infrastructure.Messaging.Sagas.States;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CentralOrganization.Api.Infrastructure.Persistence.Configurations;

public sealed class EmployeeIdentityProvisioningStateConfiguration
    : IEntityTypeConfiguration<EmployeeIdentityProvisioningState>
{
    public void Configure(EntityTypeBuilder<EmployeeIdentityProvisioningState> builder)
    {
        builder.ToTable("EmployeeIdentityProvisioningSagas");
        builder.HasKey(x => x.CorrelationId);
        builder.Property(x => x.CurrentState).HasMaxLength(100).IsRequired();
        builder.Property(x => x.EmployeeId).IsRequired();
        builder.Property(x => x.NationalCode).HasMaxLength(10).IsRequired();
        builder.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.LastName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(256).IsRequired();
        builder.Property(x => x.MobileNumber).HasMaxLength(20).IsRequired();
        builder.Property(x => x.FailureReason).HasMaxLength(1000);
        builder.HasIndex(x => x.EmployeeId).IsUnique();
    }
}