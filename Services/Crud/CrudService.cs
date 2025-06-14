namespace PharmaBack.Services.Crud;

using Microsoft.EntityFrameworkCore;
using PharmaBack.Data;

public sealed class CrudService<TEntity, TKey>(PharmaDbContext db) : ICrudService<TEntity, TKey>
    where TEntity : class
{
    private readonly DbSet<TEntity> _set = db.Set<TEntity>();

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct) =>
        await _set.AsNoTracking().ToListAsync(ct);

    public async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken ct) =>
        await _set.FindAsync([id], ct);

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken ct)
    {
        await _set.AddAsync(entity, ct);
        await db.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<bool> UpdateAsync(TKey id, TEntity entity, CancellationToken ct)
    {
        if (await _set.FindAsync([id], ct) is not { } existing)
            return false;
        db.Entry(existing).CurrentValues.SetValues(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(TKey id, CancellationToken ct)
    {
        if (await _set.FindAsync([id], ct) is not { } existing)
            return false;
        _set.Remove(existing);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
