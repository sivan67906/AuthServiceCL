using AuthService.Application.Common;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace AuthService.API.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred");

        var response = exception switch
        {
            ValidationException validationException => ApiResponse<object>.FailResponse(
                "Validation failed",
                validationException.Errors.Select(e => e.ErrorMessage).ToList()
            ),
            UnauthorizedAccessException => ApiResponse<object>.FailResponse(
                "Unauthorized access",
                new List<string> { "You are not authorized to perform this action" }
            ),
            _ => ApiResponse<object>.FailResponse(
                "An error occurred while processing your request",
                new List<string> { exception.Message }
            )
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = exception switch
        {
            ValidationException => (int)HttpStatusCode.BadRequest,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}
