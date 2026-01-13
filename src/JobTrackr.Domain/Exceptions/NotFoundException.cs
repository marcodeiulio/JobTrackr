namespace JobTrackr.Domain.Exceptions;

/// <summary>
///     Exception thrown when a requested entity cannot be found.
/// </summary>
public class NotFoundException : DomainException
{
    public NotFoundException(string entityName, object key) : base(
        $"{entityName} with id {key} was not found.")
    {
    }
}