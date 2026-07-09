using MassTransit;
using SharedKernel.Contracts.Integration.Abstractions;
using SharedKernel.Messaging.MassTransit.Consumers;

namespace SharedKernel.Messaging.MassTransit.Extensions;

public static class BusRegistrationConfiguratorExtensions
{
    public static IBusRegistrationConfigurator AddIntegrationEventConsumer<TIntegrationEvent>(
        this IBusRegistrationConfigurator configurator,
        string? endpointName = null)
        where TIntegrationEvent : IntegrationEvent
    {
        var consumerConfigurator =
            configurator.AddConsumer<MassTransitIntegrationEventConsumer<TIntegrationEvent>>();

        if (!string.IsNullOrWhiteSpace(endpointName))
        {
            consumerConfigurator.Endpoint(endpointConfigurator =>
            {
                endpointConfigurator.Name = endpointName;
            });
        }

        return configurator;
    }
}