using Scalar.AspNetCore;
using SharedKernel.Persistence.Database;

namespace File.Api.Common.Extensions;

public static class ApplicationBuilderExtensions
{
    public static async Task<WebApplication> UseIdentityPipeline(this WebApplication app)
    {
        await app.Services.ApplyMigrationsAsync<FileDbContext>();

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