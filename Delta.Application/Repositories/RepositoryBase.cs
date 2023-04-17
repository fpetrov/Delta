using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Delta.Application.Repositories;

public abstract class RepositoryBase<TEntity, TContext> : IRepository<TEntity>
    where TEntity : class
    where TContext : DbContext
{
    private TContext DatabaseContext { get; }
        
        protected RepositoryBase(TContext databaseContext)
        {
            DatabaseContext = databaseContext;
        }

        public virtual async Task<TEntity?> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await DatabaseContext
                .Set<TEntity>()
                .FindAsync(new object[] { id }, cancellationToken: cancellationToken);

            return entity;
        }

        public virtual async Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var entity = await DatabaseContext
                .Set<TEntity>()
                .FirstOrDefaultAsync(predicate, cancellationToken);

            return entity;
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var entities = await DatabaseContext
                .Set<TEntity>()
                .ToListAsync(cancellationToken);

            return entities;
        }
        
        public virtual async Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var entities = await DatabaseContext
                .Set<TEntity>()
                .Where(predicate)
                .ToListAsync(cancellationToken);

            return entities;
        }

        public virtual async Task<TEntity?> AddAsync(TEntity entity, Expression<Func<TEntity, bool>>? duplicatePredicate = default, CancellationToken cancellationToken = default)
        {
            if (duplicatePredicate != default)
            {
                var duplicate = await FindAsync(duplicatePredicate, cancellationToken);
                
                if (duplicate != null)
                    return default;
            }
            
            var createdEntity = DatabaseContext
                .Set<TEntity>()
                .Add(entity);

            await DatabaseContext.SaveChangesAsync(cancellationToken);

            return createdEntity.Entity;
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            DatabaseContext
                .Set<TEntity>()
                .AddRange(entities);

            await DatabaseContext.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            DatabaseContext
                .Set<TEntity>()
                .Update(entity);

            await DatabaseContext.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            DatabaseContext
                .Set<TEntity>()
                .UpdateRange(entities);

            await DatabaseContext.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            DatabaseContext
                .Set<TEntity>()
                .Remove(entity);

            await DatabaseContext.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            DatabaseContext
                .Set<TEntity>()
                .RemoveRange(entities);

            await DatabaseContext.SaveChangesAsync(cancellationToken);
        }
}