using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Api.Contracts;
using SharedKernel.Domain.Error;
using SharedKernel.Domain.Result;

namespace SharedKernel.Api.Extensions;

// ReSharper disable once ConvertToExtensionBlock
public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result)
        => result.IsSuccess ? new OkObjectResult(ApiResponse.Success()) : CreateProblemDetails(result.Error);

    public static IActionResult ToActionResult<T>(this Result<T> result)
        => result.IsSuccess
            ? new OkObjectResult(ApiResponse<T>.Success(result.Data))
            : CreateProblemDetails(result.Error);

    public static IActionResult ToActionResult<T, TResponse>(this Result<T> result)
    {
        if (!result.IsSuccess)
            return CreateProblemDetails(result.Error);

        var responseData = result.Data.Adapt<TResponse>();
        return new OkObjectResult(ApiResponse<TResponse>.Success(responseData));
    }

    public static IResult ToHttpResult(this Result result)
        => result.IsSuccess
            ? Results.Ok(ApiResponse.Success())
            : CreateProblem(result.Error);

    // ReSharper disable once ConvertToExtensionBlock
    public static IResult ToHttpResult<T>(this Result<T> result)
        => result.IsSuccess
            ? Results.Ok(ApiResponse<T>.Success(result.Data))
            : CreateProblem(result.Error);

    public static IResult ToHttpResult<T, TResponse>(this Result<T> result)
    {
        if (!result.IsSuccess)
            return CreateProblem(result.Error);

        var responseData = result.Data.Adapt<TResponse>();
        return Results.Ok(ApiResponse<TResponse>.Success(responseData));
    }

    private static ObjectResult CreateProblemDetails(Error error)
    {
        var statusCode = error.GetStatusCode();
        return new ObjectResult(new ApiProblemDetails
        {
            Title = error.Code,
            Detail = error.Description,
            Status = statusCode,
            Path = error.Path,
            Metadata = error.Metadata
        })
        {
            StatusCode = statusCode
        };
    }

    private static IResult CreateProblem(Error error)
    {
        var statusCode = error.GetStatusCode();

        return Results.Problem(
            title: error.Code,
            detail: error.Description,
            statusCode: statusCode,
            extensions: new Dictionary<string, object?>
            {
                ["path"] = error.Path,
                ["metadata"] = error.Metadata
            });
    }
}