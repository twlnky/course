using System.Text.Json;
using CourseBank.dto.response;
using Microsoft.AspNetCore.Diagnostics;

namespace CourseBank.services;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, message) = exception switch
        {
            AppException appEx => (appEx.StatusCode, appEx.Message),
            BadHttpRequestException badRequest => (StatusCodes.Status400BadRequest, badRequest.Message),
            JsonException => (StatusCodes.Status400BadRequest,
                "Некорректный JSON. Enum передавайте строкой: Client, RUB, Current, Pending и т.д."),
            _ => (StatusCodes.Status500InternalServerError, "Internal server error")
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
            Console.WriteLine($"[ERROR] {exception.GetType().Name}: {exception.Message}");

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        var json = JsonSerializer.Serialize(new ErrorResponse { Message = message },
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        await httpContext.Response.WriteAsync(json, cancellationToken);
        return true;
    }
}
