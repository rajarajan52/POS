namespace POSSystem.Domain.Application.Exceptions;

/// <summary>
/// Exception thrown when a requested resource is not found.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public static NotFoundException For<T>(Guid id)
        => new($"{typeof(T).Name} with ID {id} was not found.");

    public static NotFoundException For<T>(string identifier)
        => new($"{typeof(T).Name} '{identifier}' was not found.");
}
