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
            _logger.LogError(
                exception,
                "An unhandled exception occurred while processing the request.");

            await HandleExceptionAsync(context, exception);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // set response content type
        context.Response.ContentType = "application/problem+json";

        // map exception types to HTTP and create ProblemDetails
        ProblemDetails problemDetails;

        switch (exception)
        {
            case NotFoundException notFoundException:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Resource not found",
                    Detail = notFoundException.Message
                };
                break;

            case ValidationException validationException:
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
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Domain error",
                    Detail = domainException.Message
                };
                break;

            default: // unhandled exceptions
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