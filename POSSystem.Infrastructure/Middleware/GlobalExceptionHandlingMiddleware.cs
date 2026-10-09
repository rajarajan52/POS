using POSSystem.Domain.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Net;

namespace POSSystem.Infrastructure.Middleware;

/// <summary>
/// Global exception handling middleware.
/// Follows Single Responsibility and centralized error handling principles.
/// </summary>
public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger logger)
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
            _logger.Error(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse
        {
            Message = exception.Message,
            StatusCode = HttpStatusCode.InternalServerError
        };

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        return exception switch
        {
            ValidationException validationEx =>
                HandleValidationException(context, validationEx, response),
            NotFoundException notFoundEx =>
                HandleNotFoundException(context, notFoundEx, response),
            BusinessRuleException businessEx =>
                HandleBusinessRuleException(context, businessEx, response),
            _ => context.Response.WriteAsJsonAsync(response)
        };
    }

    private static Task HandleValidationException(
        HttpContext context,
        ValidationException exception,
        ErrorResponse response)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        response.StatusCode = HttpStatusCode.BadRequest;
        response.Errors = exception.Errors?.ToList();
        return context.Response.WriteAsJsonAsync(response);
    }

    private static Task HandleNotFoundException(
        HttpContext context,
        NotFoundException exception,
        ErrorResponse response)
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        response.StatusCode = HttpStatusCode.NotFound;
        return context.Response.WriteAsJsonAsync(response);
    }

    private static Task HandleBusinessRuleException(
        HttpContext context,
        BusinessRuleException exception,
        ErrorResponse response)
    {
        context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
        response.StatusCode = HttpStatusCode.UnprocessableEntity;
        response.Code = exception.Code;
        return context.Response.WriteAsJsonAsync(response);
    }
}

public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public HttpStatusCode StatusCode { get; set; }
    public string? Code { get; set; }
    public List<string>? Errors { get; set; }
}


