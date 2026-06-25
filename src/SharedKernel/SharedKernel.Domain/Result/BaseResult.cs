namespace SharedKernel.Domain.Result;

public abstract class BaseResult
{
    protected BaseResult(bool isSuccess, Error.Error error)
    {
        if ((isSuccess && error != Domain.Error.Error.None) || (!isSuccess && error == Domain.Error.Error.None))
            throw new ArgumentException($"Invalid error. Is Success: {isSuccess.ToString()}, Error: [{Error?.Code}] {Error?.Description}", nameof(error));

        IsSuccess = isSuccess;
        Error = error;
    }

    // ReSharper disable once MemberCanBeProtected.Global
    public virtual bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    // ReSharper disable once MemberCanBePrivate.Global
    public Error.Error Error { get; }
}