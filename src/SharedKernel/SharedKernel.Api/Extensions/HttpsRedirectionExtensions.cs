using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using SharedKernel.Api.Options;

namespace SharedKernel.Api.Extensions;

public static class HttpsRedirectionExtensions
{
    public static IApplicationBuilder UseHttpsRedirectionExceptPaths(this IApplicationBuilder app, Action<HttpsRedirectionExclusionOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(app);

        var options = new HttpsRedirectionExclusionOptions();
        configure?.Invoke(options);

        var excludedPaths = options.ExcludedPathPrefixes
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .Select(CreatePathString)
            .Distinct()
            .ToArray();

        app.UseWhen(
            context => !IsExcluded(context.Request.Path, excludedPaths),
            branch => branch.UseHttpsRedirection());

        return app;
    }

    private static bool IsExcluded(PathString requestPath, IReadOnlyCollection<PathString> excludedPaths)
        => excludedPaths.Any(requestPath.StartsWithSegments);

    private static PathString CreatePathString(string path)
    {
        if (!path.StartsWith('/'))
            throw new ArgumentException($"Excluded path '{path}' must start with '/'.", nameof(path));

        return new PathString(path);
    }
}