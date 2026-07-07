using File.Api.Infrastructure.Grpc;
using Scalar.AspNetCore;
using SharedKernel.Persistence.Database;

namespace File.Api.Common.Extensions;

public static class ApplicationBuilderExtensions
{
    public static async Task<WebApplication> UseFilePipeline(this WebApplication app)
    {
        await app.Services.ApplyMigrationsAsync<FileDbContext>();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseExceptionHandler();
        app.MapGrpcService<FileValidationGrpcService>();
        app.MapCarter();

        return app;
    }
}