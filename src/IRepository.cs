using System.Linq.Expressions;

namespace AppLogger;

public interface IRepository<TEntity>
{
	Task<bool> ExistAsync(long id, CancellationToken cancellation = default);
	Task<bool> SaveAsync(CancellationToken cancellation = default);
	Task<bool> DeleteAsync(long id, CancellationToken cancellation = default);
	Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellation = default);
	Task<TEntity> AddAsync(TEntity newEntity, CancellationToken cancellation = default);
	Task<TEntity> SelectAsync(Expression<Func<TEntity, bool>> expression, string[] includes = null, CancellationToken cancellation = default);
	Task<IQueryable<TEntity>> SelectAllAsync(Expression<Func<TEntity, bool>> expression, string[] includes = null, CancellationToken cancellation = default);
}
