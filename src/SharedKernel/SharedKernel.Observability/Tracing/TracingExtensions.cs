using Microsoft.AspNetCore.Http;
using OpenTelemetry.Exporter;
using OpenTelemetry.Trace;
using SharedKernel.Observability.Options;

namespace SharedKernel.Observability.Tracing;

internal static class TracingExtensions
{
    public static TracerProviderBuilder ConfigureApplicationTracing(this TracerProviderBuilder tracing,
        string serviceName,
        TracingOptions tracingOptions,
        MetricsOptions metricsOptions)
    {
        var endpoint = new Uri(tracingOptions.Endpoint, UriKind.Absolute);
        var excludedPaths = BuildExcludedPaths(tracingOptions, metricsOptions);

        return tracing
            .SetSampler(new ParentBasedSampler(new TraceIdRatioBasedSampler(tracingOptions.SamplingRatio)))
            .AddSource(serviceName)
            .AddAspNetCoreInstrumentation(instrumentation =>
            {
                instrumentation.RecordException = true;
                instrumentation.Filter = context => !IsExcluded(context.Request.Path, excludedPaths);
            })
            .AddHttpClientInstrumentation(instrumentation => instrumentation.RecordException = true)
            .AddOtlpExporter(exporter =>
            {
                exporter.Endpoint = endpoint;
                exporter.Protocol = OtlpExportProtocol.HttpProtobuf;
                exporter.Compression = OtlpExportCompression.GZip;
            });
    }

    private static PathString[] BuildExcludedPaths(TracingOptions tracingOptions, MetricsOptions metricsOptions)
    {
        var paths = (tracingOptions.ExcludedPathPrefixes ?? [])
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .Select(path => new PathString(path));

        if (metricsOptions.Enabled)
            paths = paths.Append(new PathString(metricsOptions.EndpointPath));

        return paths.Distinct().ToArray();
    }

    private static bool IsExcluded(PathString requestPath, IReadOnlyCollection<PathString> excludedPaths)
        => excludedPaths.Any(requestPath.StartsWithSegments);
}
