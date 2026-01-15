using System.Text.Json;
using JobTrackr.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace JobTrackr.API.Middleware;

/// <summary>
///     Global exception handler that catches all exception and converts them to ProblemDetails responses.
///     Runs first in the middleware pipeline to catch exceptions from all subsequent middleware.
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly RequestDelegate _next;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger)
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
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // set response content type
        context.Response.ContentType = "application/problem+json";

        // map exception types to HTTP and create ProblemDetails
        ProblemDetails problemDetails;

        switch (exception)
        {
            case NotFoundException notFoundException:
                _logger.LogWarning(
                    notFoundException,
                    "Resource not found: {EntityName} with {Key}",
                    notFoundException.EntityName,
                    notFoundException.Key);

                context.Response.StatusCode = StatusCodes.Status404NotFound;
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Resource not found",
                    Detail = notFoundException.Message
                };
                break;

            case ValidationException validationException:
                var errorMessages =
                    validationException.Errors
                        .SelectMany(e => e.Value.Select(msg => $"{e.Key}: {msg}"));

                _logger.LogWarning(validationException,
                    "Validation failed {ErrorMessages}",
                    string.Join(". ", errorMessages));

                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Validation failed",
                    Detail = validationException.Message
                };

                // add validation errors as extensions
                foreach (var error in validationException.Errors) problemDetails.Extensions[error.Key] = error.Value;

                break;

            case DomainException domainException:
                _logger.LogWarning(
                    domainException,
                    "Domain exception occurred: {Message}",
                    domainException.Message);

                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Domain error",
                    Detail = domainException.Message
                };
                break;

            default: // unhandled exceptions
                _logger.LogError(
                    exception,
                    "Unhandled exception of type {ExceptionName}: {Message}",
                    exception.GetType().Name,
                    exception.Message);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Internal server error",
                    Detail = "An unexpected error occurred. Please try again later."
                };
                break;
        }

        // add trace ID for debugging
        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        // serialize and write response
        var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}