namespace WorkoutTracker.Models;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? ErrorMessage { get; }
    public ResultType Type { get; }
    /// <param name="success">Whether the operation succeeded.</param>
    /// <param name="errorMessage">The error message if it failed.</param>
    /// <param name="type">The type of result.</param>
    protected Result(bool success, string? errorMessage, ResultType type)
    {
        IsSuccess = success;
        ErrorMessage = errorMessage;
        Type = type;
    }

    public static Result Success() => new(true, null, ResultType.Ok);

    public static Result Failure(string message, ResultType type = ResultType.BadRequest) => new(false, message, type);

    public static Result NotFound(string message = "Resource not found") => new(false, message, ResultType.NotFound);

    public static Result Forbidden(string message = "Access denied") => new(false, message, ResultType.Forbidden);
}

public class Result<T> : Result
{
    public T? Value { get; }

    private Result(bool success, T? value, string? errorMessage, ResultType type) 
        : base(success, errorMessage, type)
    {
        Value = value;
    }
    public static Result<T> Success(T value) => new(true, value, null, ResultType.Ok);

    public new static Result<T> Failure(string message, ResultType type = ResultType.BadRequest) => new(false, default, message, type);

    public new static Result<T> NotFound(string message = "Resource not found") => new(false, default, message, type: ResultType.NotFound);

    public new static Result<T> Forbidden(string message = "Access denied") => new(false, default, message, type: ResultType.Forbidden);
}

public enum ResultType
{

    Ok,
    BadRequest,
    NotFound,
    Forbidden,
    Conflict,
    Error
}
