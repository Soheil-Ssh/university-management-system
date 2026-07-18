namespace SharedKernel.Observability.Correlation;

public static class CorrelationIdConstants
{
    public const string HeaderName = "X-Correlation-Id";
    public const string LogPropertyName = "CorrelationId";
    public const string HttpContextItemName = "Observability.CorrelationId";
}