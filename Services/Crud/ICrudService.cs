namespace PharmaBack.Services.Crud;

public interface ICrudService<TEntity, TKey>
    where TEntity : class
{
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct);
    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken ct);
    Task<TEntity> AddAsync(TEntity entity, CancellationToken ct);
    Task<bool> UpdateAsync(TKey id, TEntity entity, CancellationToken ct);
    Task<bool> DeleteAsync(TKey id, CancellationToken ct);
}
