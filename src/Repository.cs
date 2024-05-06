using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AppLogger;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : AudiTable
{
	private readonly DbContext dbContext;
	private readonly DbSet<TEntity> dbSet;
	public Repository(DbContext dbContext)
    {
		this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		dbSet =	dbContext.Set<TEntity>();

    }
	public async Task<TEntity> AddAsync(TEntity newEntity, CancellationToken cancellation = default)
		=> (await dbSet.AddAsync(newEntity).ConfigureAwait(false)).Entity;

	public async Task<bool> DeleteAsync(long id, CancellationToken cancellation = default)
	{
		var entity = await dbSet
			.FirstOrDefaultAsync(x => x.Id == id, cancellation)
			.ConfigureAwait(false);

		dbSet.Remove(entity);
		return true;
	}

	public async Task<bool> ExistAsync(long id, CancellationToken cancellation = default)
		=> await dbSet.AnyAsync(x => x.Id == id, cancellation);

	public async Task<bool> SaveAsync(CancellationToken cancellation = default)
		=> await dbContext.SaveChangesAsync(cancellation).ConfigureAwait(false) > 0;

	public Task<IQueryable<TEntity>> SelectAllAsync(Expression<Func<TEntity, bool>> expression, string[] includes = null, CancellationToken cancellation = default)
	{
		return Task.Run(() =>
		{
			var query = expression is null ? dbSet : dbSet.Where(expression);
			if (includes is not null)
				foreach (var include in includes)
					query = query.Include(include);

			return query;
		}, cancellation);
	}

	public async Task<TEntity> SelectAsync(Expression<Func<TEntity, bool>> expression, string[] includes = null, CancellationToken cancellation = default)
	{
		var query = await SelectAllAsync(expression, includes, cancellation).ConfigureAwait(false);
		return await query.FirstOrDefaultAsync(expression, cancellation).ConfigureAwait(false);
	}

	public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellation = default)
		=> Task.Run(() => dbSet.Update(entity).Entity, cancellation);
}
