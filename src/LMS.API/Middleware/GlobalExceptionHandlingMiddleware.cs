using LMS.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace LMS.API.Middleware;

/// <summary>
/// Global exception handling middleware.
/// Catches unhandled exceptions and returns RFC 7807 ProblemDetails responses.
/// Keeps controllers clean — no try/catch needed.
/// </summary>
public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title) = exception switch
        {
            NotFoundException => (HttpStatusCode.NotFound, "Resource Not Found"),
            BusinessRuleException => (HttpStatusCode.BadRequest, "Business Rule Violation"),
            UnauthorizedException => (HttpStatusCode.Unauthorized, "Unauthorized"),
            ValidationException => (HttpStatusCode.UnprocessableEntity, "Validation Error"),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred")
        };

        var problem = new ProblemDetails
        {
            Title = title,
            Detail = exception.Message,
            Status = (int)statusCode
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(problem);
        await context.Response.WriteAsync(json);
    }
}
