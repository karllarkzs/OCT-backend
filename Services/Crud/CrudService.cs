using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PharmaBack.Data;
using PharmaBack.Models;

namespace PharmaBack.Services.Crud;

public sealed class CrudService<TEntity, TKey>(PharmaDbContext db) : ICrudService<TEntity, TKey>
    where TEntity : class
{
    private readonly DbSet<TEntity> _set = db.Set<TEntity>();

    public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>>? predicate = null)
    {
        var q = _set.AsNoTracking();
        return predicate is null ? q : q.Where(predicate);
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default) =>
        await Query().ToListAsync(ct);

    public ValueTask<TEntity?> GetByIdAsync(TKey id, CancellationToken ct = default) =>
        _set.FindAsync([id], ct);

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken ct = default)
    {
        await _set.AddAsync(entity, ct);
        await db.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<bool> UpdateAsync(
        TKey id,
        Action<TEntity> mutate,
        CancellationToken ct = default
    )
    {
        var existing = await _set.FindAsync([id], ct);
        if (existing is null)
            return false;

        mutate(existing);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(TKey id, CancellationToken ct = default)
    {
        var existing = await _set.FindAsync([id], ct);
        if (existing is null)
            return false;

        if (existing is ISoftDelete soft)
        {
            soft.IsDeleted = true;
        }
        else
        {
            _set.Remove(existing);
        }

        await db.SaveChangesAsync(ct);
        return true;
    }
}
