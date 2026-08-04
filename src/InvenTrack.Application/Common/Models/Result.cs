namespace InvenTrack.Application.Common.Models;

public class Result
{
    public bool Succeeded { get; set; }
    public string[] Errors { get; set; } = Array.Empty<string>();

    public static Result Success() => new() { Succeeded = true };

    public static Result Failure(IEnumerable<string> errors) => new()
    {
        Succeeded = false,
        Errors = errors.ToArray()
    };

    public static Result Failure(string error) => new()
    {
        Succeeded = false,
        Errors = new[] { error }
    };
}

public class Result<T>
{
    public bool Succeeded { get; set; }
    public T? Data { get; set; }
    public string[] Errors { get; set; } = Array.Empty<string>();

    public static Result<T> Success(T data) => new() { Succeeded = true, Data = data };

    public static Result<T> Failure(IEnumerable<string> errors) => new()
    {
        Succeeded = false,
        Errors = errors.ToArray()
    };

    public static Result<T> Failure(string error) => new()
    {
        Succeeded = false,
        Errors = new[] { error }
    };
}
