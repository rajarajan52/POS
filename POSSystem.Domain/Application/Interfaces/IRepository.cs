using POSSystem.Domain.Domain.Models;

namespace POSSystem.Domain.Application.Interfaces;

/// <summary>
/// Generic repository interface following SOLID principles (Interface Segregation).
/// </summary>
/// <typeparam name="TEntity">The aggregate root type</typeparam>
/// <typeparam name="TId">The type of the entity's ID</typeparam>
public interface IRepository<TEntity, TId> where TEntity : AggregateRoot
{
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default);
}


