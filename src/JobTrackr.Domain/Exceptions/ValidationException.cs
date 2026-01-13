namespace JobTrackr.Domain.Exceptions;

/// <summary>
///     Exception thrown when domain validation rules are violated.
/// </summary>
public class ValidationException : DomainException
{
    public ValidationException() : base("One or more validation errors have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(Dictionary<string, string[]> errors) : base(
        "One or more validation errors have occurred.")
    {
        Errors = errors;
    }

    public ValidationException(string propertyName, string errorMessage) : base(
        "One or more validation errors have occurred.")

    {
        Errors = new Dictionary<string, string[]>
        {
            { propertyName, [errorMessage] }
        };
    }

    public IDictionary<string, string[]> Errors { get; }
}