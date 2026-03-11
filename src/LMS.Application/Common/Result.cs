namespace LMS.Application.Common;

/// <summary>
/// Encapsulates the result of an application service operation.
/// Follows the Result Pattern to avoid throwing exceptions for predictable failures.
/// </summary>
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public string? ErrorMessage { get; }
    public int StatusCode { get; }

    private Result(bool isSuccess, T? data, string? errorMessage, int statusCode)
    {
        IsSuccess = isSuccess;
        Data = data;
        ErrorMessage = errorMessage;
        StatusCode = statusCode;
    }

    public static Result<T> Success(T data) => new(true, data, null, 200);
    public static Result<T> Created(T data) => new(true, data, null, 201);
    public static Result<T> NotFound(string message) => new(false, default, message, 404);
    public static Result<T> BadRequest(string message) => new(false, default, message, 400);
    public static Result<T> Unauthorized(string message = "Access denied.") => new(false, default, message, 401);
    public static Result<T> Conflict(string message) => new(false, default, message, 409);
    public static Result<T> Failure(string message, int statusCode = 500) => new(false, default, message, statusCode);
}

/// <summary>Non-generic result for void operations.</summary>
public class Result
{
    public bool IsSuccess { get; }
    public string? ErrorMessage { get; }
    public int StatusCode { get; }

    private Result(bool isSuccess, string? errorMessage, int statusCode)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        StatusCode = statusCode;
    }

    public static Result Success() => new(true, null, 200);
    public static Result NotFound(string message) => new(false, message, 404);
    public static Result BadRequest(string message) => new(false, message, 400);
    public static Result Unauthorized(string message = "Access denied.") => new(false, message, 401);
    public static Result Failure(string message, int statusCode = 500) => new(false, message, statusCode);
}
