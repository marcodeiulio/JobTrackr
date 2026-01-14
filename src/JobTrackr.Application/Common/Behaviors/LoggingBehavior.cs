using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace JobTrackr.Application.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        // @ symbol tells Serilog to destructure the object, making it searchable
        logger.LogInformation("Executing {RequestName}: {@Request}", requestName, request);

        var stopwatch = Stopwatch.StartNew();
        var response = await next(cancellationToken);
        stopwatch.Stop();

        logger.LogInformation("Completed {RequestName} in {ElapsedMilliseconds}ms.", requestName,
            stopwatch.ElapsedMilliseconds);

        return response;
    }
}