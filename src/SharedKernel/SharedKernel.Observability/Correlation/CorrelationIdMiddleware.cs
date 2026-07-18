using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Serilog.Context;
using System.Diagnostics;

namespace SharedKernel.Observability.Correlation;

public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    private const int MaximumCorrelationIdLength = 128;

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetOrCreateCorrelationId(context);
        context.Items[CorrelationIdConstants.HttpContextItemName] = correlationId;
        Activity.Current?.SetTag("correlation.id", correlationId);

        context.Response.OnStarting(() =>
        {
            context.Response.Headers[CorrelationIdConstants.HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        using (LogContext.PushProperty(CorrelationIdConstants.LogPropertyName, correlationId))
        using (LogContext.PushProperty("RequestId", context.TraceIdentifier))
        {
            await next(context);
        }
    }

    private static string GetOrCreateCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(CorrelationIdConstants.HeaderName, out StringValues values))
        {
            var value = values.FirstOrDefault()?.Trim();

            if (!string.IsNullOrWhiteSpace(value) && value.Length <= MaximumCorrelationIdLength)
                return value;
        }

        return Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;
    }
}