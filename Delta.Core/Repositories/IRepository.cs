using System.Linq.Expressions;

namespace Delta.Core.Repositories;

/// <summary>
/// Interface for repository classes.
/// </summary>
/// <typeparam name="TEntity">Repository entity.</typeparam>
public interface IRepository<TEntity>
    where TEntity : class
{
    public Task<TEntity?> GetAsync(int id,
        CancellationToken cancellationToken = default);
    
    public Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);
    
    public Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    
    public Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    public Task<TEntity?> AddAsync(TEntity entity,
        Expression<Func<TEntity, bool>>? duplicatePredicate = default,
        CancellationToken cancellationToken = default);

    public Task AddRangeAsync(IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default);

    public Task UpdateAsync(TEntity entity,
        CancellationToken cancellationToken = default);
    
    public Task UpdateRangeAsync(IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default);

    public Task RemoveAsync(TEntity entity,
        CancellationToken cancellationToken = default);

    public Task RemoveRangeAsync(IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default);
}