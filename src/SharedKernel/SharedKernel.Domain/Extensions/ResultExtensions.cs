using SharedKernel.Domain.Result;

namespace SharedKernel.Domain.Extensions;

public static class ResultExtensions
{
    public static Result.Result WithPath(this Result.Result result, string path)
        => result.IsSuccess ? result : Result.Result.Failure(result.Error.WithPath(path));

    public static Result<T> WithPath<T>(this Result<T> result, string path)
        => result.IsSuccess ? result : Result<T>.Failure(result.Error.WithPath(path));
}