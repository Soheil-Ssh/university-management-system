using Asp.Versioning;

namespace File.Api.Common.Extensions;

public static class VersioningExtensions
{
    public static RouteHandlerBuilder Version(
        this RouteHandlerBuilder builder,
        IEndpointRouteBuilder app,
        double version)
    {
        var apiVersion = new ApiVersion(version);

        var set = app.NewApiVersionSet()
            .HasApiVersion(apiVersion)
            .ReportApiVersions()
            .Build();

        return builder
            .WithApiVersionSet(set)
            .MapToApiVersion(apiVersion);
    }
}