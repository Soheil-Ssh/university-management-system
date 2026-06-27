using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SharedKernel.Api.Contracts;

namespace SharedKernel.Api.ExceptionHandling;

public class ValidationExceptionHandler(ILogger<ValidationExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ValidationException ex)
            return false;

        logger.LogWarning(ex, "Validation error occurred");

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        await httpContext.Response.WriteAsJsonAsync(
            new ApiProblemDetails
            {
                Title = "Validation Failed",
                Status = StatusCodes.Status400BadRequest,
                Detail = "One or more validation errors occurred",
                TraceId = httpContext.TraceIdentifier,
                Errors = ex.Errors.Select(x => new ValidationError(x.PropertyName, x.ErrorMessage))
            },
            cancellationToken);

        return true;
    }
}