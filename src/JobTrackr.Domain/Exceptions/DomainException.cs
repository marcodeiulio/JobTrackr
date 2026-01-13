namespace JobTrackr.Domain.Exceptions;

/// <summary>
///     Base exception for all domain-specific errors.
///     Inherit from this for specific domain violations.
/// </summary>
public class DomainException : Exception
{
    public DomainException()
    {
    }

    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}