namespace SharedKernel.Messaging.MassTransit.Enums;

public enum MessagingOutboxProvider
{
    None = 0,
    Postgres = 1,
    SqlServer = 2,
}