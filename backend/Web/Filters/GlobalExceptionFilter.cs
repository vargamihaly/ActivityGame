using System.Net;
using ActivityGameBackend.Application.Exceptions;
using ActivityGameBackend.Web.Shared;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ActivityGameBackend.Web.Filters;

public class GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        ApiResponse response;
        HttpStatusCode statusCode;

        switch (context.Exception)
        {
            case AppException appEx:
                statusCode = appEx.StatusCode;
                response = new ApiResponse
                {
                    Success = false,
                    Message = appEx.Message,
                    ErrorCode = appEx.ErrorCode,
                };
                logger.LogWarning(context.Exception, "Application exception occurred.");
                break;
            default:
                statusCode = HttpStatusCode.InternalServerError;
                response = new ApiResponse
                {
                    Success = false,
                    Message = context.Exception.Message,
                    ErrorCode = 0, // Generic error code for unhandled exceptions
                };
                logger.LogError(context.Exception, "Unhandled exception");
                break;
        }

        context.Result = new ObjectResult(response)
        {
            StatusCode = (int)statusCode,
        };

        context.ExceptionHandled = true;
    }
}
