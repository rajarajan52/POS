using POSSystem.Domain.Domain.Models;

namespace POSSystem.Domain.Application.Interfaces;

/// <summary>
/// Unit of Work pattern to manage transactions across multiple repositories.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<T> BeginTransactionAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default);
}


