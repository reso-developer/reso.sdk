using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Reso.Sdk.Core.BaseConnect
{
	public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
	{
		protected DbContext dbContext;

		protected DbSet<TEntity> dbSet;

		public BaseRepository(DbContext dbContext)
		{
			this.dbContext = dbContext;
			dbSet = this.dbContext.Set<TEntity>();
		}

		public async Task<IDbContextTransaction> BeginTransaction(CancellationToken cancellationToken = default(CancellationToken))
		{
			return await dbContext.Database.BeginTransactionAsync(cancellationToken);
		}

		public void AddRange(IEnumerable<TEntity> entities)
		{
			dbSet.AddRange(entities);
		}

		public async Task AddRangeAsyn(IEnumerable<TEntity> entities)
		{
			await dbSet.AddRangeAsync(entities);
		}

		public int Count()
		{
			return dbSet.Count();
		}

		public void Create(TEntity entity)
		{
			dbSet.Add(entity);
		}

		public async Task CreateAsyn(TEntity entity)
		{
			await dbSet.AddAsync(entity);
		}

		public void Delete(TEntity entity)
		{
			dbSet.Remove(entity);
		}

		public TEntity FirstOrDefault()
		{
			return dbSet.FirstOrDefault();
		}

		public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
		{
			return dbSet.FirstOrDefault(predicate);
		}

		public Task<TEntity> FirstOrDefaultAsyn()
		{
			return dbSet.FirstOrDefaultAsync();
		}

		public Task<TEntity> FirstOrDefaultAsyn(Expression<Func<TEntity, bool>> predicate)
		{
			return dbSet.FirstOrDefaultAsync(predicate);
		}

		public TEntity Get<TKey>(TKey id)
		{
			return dbSet.Find(id);
		}

		public IQueryable<TEntity> Get()
		{
			return dbSet;
		}

		public IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate)
		{
			return dbSet.Where(predicate);
		}

		public async Task<TEntity> GetAsyn<TKey>(TKey id)
		{
			return await dbSet.FindAsync(id);
		}

		public void RemoveRange(IEnumerable<TEntity> entities)
		{
			dbSet.RemoveRange(entities);
		}

		public void Update(TEntity entity)
		{
			dbSet.Update(entity);
		}
	}
}
