namespace SharedKernel.Domain.Result;

public class Result : BaseResult
{
    private Result(bool isSuccess, Error.Error error)
        : base(isSuccess, error) { }

    public static Result Success() => new(true, Domain.Error.Error.None);

    // ReSharper disable once MemberCanBePrivate.Global
    public static Result Failure(Error.Error error) => new(false, error);

    public static implicit operator Result(Error.Error error) => Failure(error);
}

public class Result<TData> : BaseResult
{
    private readonly TData? _data;

    private Result(TData? data, bool isSuccess, Error.Error error) : base(isSuccess, error)
    {
        _data = data;
        IsSuccess = isSuccess;
    }

    public TData Data => IsSuccess
        ? _data!
        : throw new InvalidOperationException("Cannot access data of a failed result.");

    public override bool IsSuccess { get; }

    // ReSharper disable once MemberCanBePrivate.Global
    public static Result<TData> Success(TData data) => new(data, true, Domain.Error.Error.None);

    // ReSharper disable once MemberCanBePrivate.Global
    public static Result<TData> Failure(Error.Error error) => new(default, false, error);

    public static implicit operator Result<TData>(TData data) => Success(data);

    public static implicit operator Result<TData>(Error.Error error) => Failure(error);
}