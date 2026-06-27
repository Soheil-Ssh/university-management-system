namespace Identity.Api.Common.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseIdentityPipeline(this IApplicationBuilder app)
    {
        return app;
    }
}