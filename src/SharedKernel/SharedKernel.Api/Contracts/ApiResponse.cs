namespace SharedKernel.Api.Contracts;

public sealed record ApiResponse(bool Succeeded, string? Message)
{
    public static ApiResponse Success(string? message = null) => new(true, message);
}

public sealed record ApiResponse<T>(T? Data, bool Succeeded, string? Message)
{
    public static ApiResponse<T> Success(T? data, string? message = null) => new(data, true, message);
}