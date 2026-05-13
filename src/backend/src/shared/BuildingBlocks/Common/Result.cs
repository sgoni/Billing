namespace BuildingBlocks.Common;

/// <summary>
///     Result Pattern
/// </summary>
/// <typeparam name="T"></typeparam>
public class Result<T>
{
    protected Result(bool isSuccess, T data, Error error)
    {
        IsSuccess = isSuccess;
        Data = data;
        Error = error;
    }

    public bool IsSuccess { get; }
    public T Data { get; }
    public Error Error { get; }

    public static Result<T> Success(T data)
    {
        return new Result<T>(true, data, null);
    }

    public static Result<T> Failure(Error error)
    {
        return new Result<T>(false, default, error);
    }
}