using Microsoft.AspNetCore.Mvc;

namespace SharedKernel.Api.Contracts;

public class ApiProblemDetails : ProblemDetails
{
    public IEnumerable<ValidationError>? Errors { get; init; }
    public string? TraceId { get; init; }
}

public sealed record ValidationError(string PropertyName, string ErrorMessage);