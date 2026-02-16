using System.Linq.Expressions;

using Server.Domain.Entities;

namespace Server.Domain.Interfaces.Repositories;

/// <summary>
///     Generic repository contract with CRUD, soft-delete, restore, and unit-of-work capabilities.
/// </summary>
public interface IBaseRepository<T> where T : BaseEntity
{
    // Read operations
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<T?> GetByIdIncludingDeletedAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllIncludingDeletedAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    // Pagination
    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    // Write operations
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    // Soft-delete operations
    Task<bool> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> SoftDeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task<int> SoftDeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    Task<int> SoftDeleteRangeAsync(Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    // Restore operations
    Task<bool> RestoreAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> RestoreAsync(T entity, CancellationToken cancellationToken = default);

    // Hard-delete operations (use with caution)
    Task<bool> HardDeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> HardDeleteAsync(T entity, CancellationToken cancellationToken = default);

    // Unit of work
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
