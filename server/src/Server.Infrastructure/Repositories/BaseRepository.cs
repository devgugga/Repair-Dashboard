using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using Server.Domain.Entities;
using Server.Domain.Interfaces.Repositories;
using Server.Infrastructure.Data;

namespace Server.Infrastructure.Repositories;

public class BaseRepository<T>(ServerDbContext context) : IBaseRepository<T> where T : BaseEntity
{
    private DbSet<T> DbSet => context.Set<T>();

    #region Unit of Work

    public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return await context.SaveChangesAsync(cancellationToken);
    }

    #endregion

    #region Private Methods

    /// <summary>
    ///     Atualiza campos de auditoria automaticamente
    /// </summary>
    private void UpdateAuditFields()
    {
        IEnumerable<EntityEntry<BaseEntity>> entries = context.ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (EntityEntry<BaseEntity> entry in entries)
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = null;
                    break;

                case EntityState.Modified:
                    entry.Property(nameof(BaseEntity.CreatedAt)).IsModified = false;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
    }

    #endregion

    #region Read Operations

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public virtual async Task<T?> GetByIdIncludingDeletedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> GetAllIncludingDeletedAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.IgnoreQueryFilters().ToListAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public virtual async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(x => x.Id == id, cancellationToken);
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(predicate, cancellationToken);
    }

    public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.CountAsync(cancellationToken);
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.CountAsync(predicate, cancellationToken);
    }

    public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = DbSet.AsQueryable();

        if (predicate != null)
            query = query.Where(predicate);

        int totalCount = await query.CountAsync(cancellationToken);

        List<T> items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    #endregion

    #region Write Operations

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        EntityEntry<T> entry = await DbSet.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities,
        CancellationToken cancellationToken = default)
    {
        var entitiesList = entities.ToList();
        await DbSet.AddRangeAsync(entitiesList, cancellationToken);
        return entitiesList;
    }

    public virtual Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        DbSet.Update(entity);
        return Task.FromResult(entity);
    }

    public virtual Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities,
        CancellationToken cancellationToken = default)
    {
        var entitiesList = entities.ToList();
        DbSet.UpdateRange(entitiesList);
        return Task.FromResult<IEnumerable<T>>(entitiesList);
    }

    #endregion

    #region Soft Delete Operations

    public virtual async Task<bool> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        T? entity = await GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return false;

        entity.SoftDelete();
        return true;
    }

    public virtual Task<bool> SoftDeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            return Task.FromResult(false);

        entity.SoftDelete();
        return Task.FromResult(true);
    }

    public virtual Task<int> SoftDeleteRangeAsync(IEnumerable<T> entities,
        CancellationToken cancellationToken = default)
    {
        var entitiesList = entities.ToList();
        foreach (T entity in entitiesList) entity.SoftDelete();
        return Task.FromResult(entitiesList.Count);
    }

    public virtual async Task<int> SoftDeleteRangeAsync(Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<T> entities = await FindAsync(predicate, cancellationToken);
        return await SoftDeleteRangeAsync(entities, cancellationToken);
    }

    #endregion

    #region Restore Operations

    public virtual async Task<bool> RestoreAsync(Guid id, CancellationToken cancellationToken = default)
    {
        T? entity = await GetByIdIncludingDeletedAsync(id, cancellationToken);
        if (entity == null || !entity.IsDeleted)
            return false;

        entity.Restore();
        return true;
    }

    public virtual Task<bool> RestoreAsync(T entity, CancellationToken cancellationToken = default)
    {
        if (entity == null || !entity.IsDeleted)
            return Task.FromResult(false);

        entity.Restore();
        return Task.FromResult(true);
    }

    #endregion

    #region Hard Delete Operations

    public virtual async Task<bool> HardDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        T? entity = await DbSet.IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity == null)
            return false;

        DbSet.Remove(entity);
        return true;
    }

    public virtual Task<bool> HardDeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            return Task.FromResult(false);

        DbSet.Remove(entity);
        return Task.FromResult(true);
    }

    #endregion
}
