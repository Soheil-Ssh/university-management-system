using Carter;
using Scalar.AspNetCore;

namespace Identity.Api.Common.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseIdentityPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseExceptionHandler();
        app.UseHttpsRedirection();
        app.MapCarter();

        return app;
    }
}