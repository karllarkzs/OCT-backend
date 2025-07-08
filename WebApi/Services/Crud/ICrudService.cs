using System.Linq.Expressions;

namespace PharmaBack.WebApi.Services.Crud;

public interface ICrudService<TEntity, TKey>
    where TEntity : class
{
    IQueryable<TEntity> Query(Expression<Func<TEntity, bool>>? predicate = null);
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default);
    ValueTask<TEntity?> GetByIdAsync(TKey id, CancellationToken ct = default);
    Task<TEntity> AddAsync(TEntity entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(TKey id, Action<TEntity> mutate, CancellationToken ct = default);
    Task<bool> DeleteAsync(TKey id, CancellationToken ct = default);
}
