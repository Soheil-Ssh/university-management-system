using Microsoft.AspNetCore.Builder;

namespace SharedKernel.Api.Extensions;

public static class HttpsRedirectionExtensions
{
    public static IApplicationBuilder UseHttpsRedirectionExceptHealthChecks(this IApplicationBuilder app, string healthCheckBaseUrl = "/health")
    {
        ArgumentNullException.ThrowIfNull(app);

        app.UseWhen(
            context => !context.Request.Path.StartsWithSegments(healthCheckBaseUrl),
            branch =>
            {
                branch.UseHttpsRedirection();
            });

        return app;
    }
}