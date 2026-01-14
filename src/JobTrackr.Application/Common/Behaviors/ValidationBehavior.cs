using FluentValidation;
using MediatR;
using ValidationException = JobTrackr.Domain.Exceptions.ValidationException;

namespace JobTrackr.Application.Common.Behaviors;

/// <summary>
///     MediatR pipeline behavior that validates requests before they reach handlers.
/// </summary>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // if no validators exist for this handler, skip validation
        if (!_validators.Any())
            return await next(cancellationToken);

        // run validators and collect results
        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll( // parallel execution
            _validators.Select(v => v.ValidateAsync(context, cancellationToken))
        );

        // collect potential failures
        var failures = validationResults
            .Where(r => !r.IsValid)
            .SelectMany(r => r.Errors)
            .ToList();

        // if any failures are found, throw ValidationException
        if (failures.Count != 0)
            throw new ValidationException(failures
                .GroupBy(f => f.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g =>
                        g.Select(f => f.ErrorMessage).ToArray()
                ));

        // if validation is successful, continue to handler
        return await next(cancellationToken);
    }
}