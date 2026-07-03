using Microsoft.AspNetCore.Mvc;

namespace SharedKernel.Api.Contracts;

public class ApiProblemDetails : ProblemDetails
{
    public IEnumerable<ValidationError>? Errors { get; init; }
    public string? TraceId { get; init; }
    public string? Path { get; init; }
    public IReadOnlyDictionary<string, object?>? Metadata { get; init; }
}

public sealed record ValidationError(string PropertyName, string ErrorMessage);