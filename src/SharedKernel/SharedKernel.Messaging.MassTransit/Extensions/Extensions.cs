using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace SharedKernel.Messaging.MassTransit.Extensions;

public static class ModelBuilderOutboxExtensions
{
    public static ModelBuilder AddMassTransitOutboxEntities(this ModelBuilder modelBuilder)
    {
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();

        return modelBuilder;
    }
}