using ActivityGameBackend.Application.Exceptions;

namespace ActivityGameBackend.Web.Shared;

public class ApiResponse
{
    public bool Success { get; init; } = true;
    public string Message { get; init; } = "Operation completed successfully";
    public ErrorCode? ErrorCode { get; init; }
}

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; init; }
}
