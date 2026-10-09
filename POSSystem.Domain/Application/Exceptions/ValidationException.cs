namespace POSSystem.Domain.Application.Exceptions;

/// <summary>
/// Validation exception for business rule violations.
/// </summary>
public class ValidationException : Exception
{
    public ValidationException(string message) : base(message)
    {
    }

    public ValidationException(string message, IEnumerable<string> errors) : base(message)
    {
        Errors = errors;
    }

    public IEnumerable<string> Errors { get; } = [];
}


