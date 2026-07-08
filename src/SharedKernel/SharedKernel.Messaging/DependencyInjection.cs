using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Messaging.Enums;
using SharedKernel.Messaging.Options;

namespace SharedKernel.Messaging;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationMessaging(this IServiceCollection services,
        IConfiguration configuration,
        Action<IBusRegistrationConfigurator>? configure = null)
    {
        var options = GetMessagingOptions(configuration);

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();

            configure?.Invoke(busConfigurator);

            busConfigurator.UsingRabbitMq((context, rabbitConfigurator) =>
            {
                ConfigureRabbitMqHost(rabbitConfigurator, options);
                rabbitConfigurator.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    public static IServiceCollection AddApplicationMessagingWithEfOutbox<TDbContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        MessagingOutboxProvider outboxProvider,
        Action<IBusRegistrationConfigurator>? configure = null)
        where TDbContext : DbContext
    {
        if (outboxProvider == MessagingOutboxProvider.None)
            return services.AddApplicationMessaging(configuration, configure);

        var options = GetMessagingOptions(configuration);

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();

            configure?.Invoke(busConfigurator);

            busConfigurator.AddEntityFrameworkOutbox<TDbContext>(outboxConfigurator =>
            {
                ConfigureOutboxProvider(outboxConfigurator, outboxProvider);
                outboxConfigurator.UseBusOutbox();
            });

            busConfigurator.UsingRabbitMq((context, rabbitConfigurator) =>
            {
                ConfigureRabbitMqHost(rabbitConfigurator, options);
                rabbitConfigurator.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    private static MessagingOptions GetMessagingOptions(IConfiguration configuration)
        => configuration.GetSection(MessagingOptions.SectionName)
            .Get<MessagingOptions>() ?? new MessagingOptions();

    private static void ConfigureRabbitMqHost(IRabbitMqBusFactoryConfigurator rabbitConfigurator,
        MessagingOptions options)
    {
        rabbitConfigurator.Host(
            options.Host,
            options.Port,
            options.VirtualHost,
            hostConfigurator =>
            {
                hostConfigurator.Username(options.Username);
                hostConfigurator.Password(options.Password);
            });
    }

    private static void ConfigureOutboxProvider(IEntityFrameworkOutboxConfigurator outboxConfigurator,
        MessagingOutboxProvider provider)
    {
        switch (provider)
        {
            case MessagingOutboxProvider.Postgres:
                outboxConfigurator.UsePostgres();
                break;

            case MessagingOutboxProvider.SqlServer:
                outboxConfigurator.UseSqlServer();
                break;

            default:
                throw new InvalidOperationException(
                    $"Unsupported messaging outbox provider: {provider}");
        }
    }
}