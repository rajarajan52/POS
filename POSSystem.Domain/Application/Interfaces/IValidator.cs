namespace POSSystem.Domain.Application.Interfaces;

/// <summary>
/// Generic validator interface for business logic validation.
/// Follows Single Responsibility and Dependency Inversion principles.
/// </summary>
/// <typeparam name="T">The type to validate</typeparam>
public interface IValidator<T>
{
    Task<ValidationResult> ValidateAsync(T value, CancellationToken cancellationToken = default);
}

public record ValidationResult(bool IsValid, IReadOnlyList<string> Errors);


