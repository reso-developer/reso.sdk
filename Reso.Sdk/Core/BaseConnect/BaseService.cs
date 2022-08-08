using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Reso.Sdk.Core.BaseConnect
{
	public class BaseService<TEntity> : IDisposable, IBaseService<TEntity> where TEntity : class
	{
		protected IUnitOfWork unitOfWork;

		protected IBaseRepository<TEntity> repository;

		public BaseService()
		{
		}

		public BaseService(IUnitOfWork unitOfWork, IBaseRepository<TEntity> repository)
		{
			this.unitOfWork = unitOfWork;
			this.repository = repository;
		}

		public void AddRange(IEnumerable<TEntity> entities)
		{
			repository.AddRange(entities);
			unitOfWork.Commit();
		}

		public async Task AddRangeAsyn(IEnumerable<TEntity> entities)
		{
			await repository.AddRangeAsyn(entities);
			await SaveAsyn();
		}

		public int Count()
		{
			return repository.Count();
		}

		public void Create(TEntity entity)
		{
			repository.Create(entity);
			Save();
		}

		public async Task CreateAsyn(TEntity entity)
		{
			await repository.CreateAsyn(entity);
			await SaveAsyn();
		}

		public void Delete(TEntity entity)
		{
			repository.Delete(entity);
			Save();
		}

		public async Task DeleteAsyn(TEntity entity)
		{
			repository.Delete(entity);
			await SaveAsyn();
		}

		public TEntity FirstOrDefault()
		{
			return repository.FirstOrDefault();
		}

		public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
		{
			return repository.FirstOrDefault(predicate);
		}

		public async Task<TEntity> FirstOrDefaultAsyn()
		{
			return await repository.FirstOrDefaultAsyn();
		}

		public async Task<TEntity> FirstOrDefaultAsyn(Expression<Func<TEntity, bool>> predicate)
		{
			return await repository.FirstOrDefaultAsyn(predicate);
		}

		public TEntity Get<TKey>(TKey id)
		{
			return repository.Get(id);
		}

		public IQueryable<TEntity> Get()
		{
			return repository.Get();
		}

		public IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate)
		{
			return repository.Get(predicate);
		}

		public async Task<TEntity> GetAsyn<TKey>(TKey id)
		{
			return await repository.GetAsyn(id);
		}

		public void RemoveRange(IEnumerable<TEntity> entities)
		{
			repository.RemoveRange(entities);
			Save();
		}

		public async Task RemoveRangeAsyn(IEnumerable<TEntity> entities)
		{
			repository.RemoveRange(entities);
			await SaveAsyn();
		}

		public void Save()
		{
			unitOfWork.Commit();
		}

		public async Task SaveAsyn()
		{
			await unitOfWork.CommitAsync();
		}

		public void Update(TEntity entity)
		{
			repository.Update(entity);
			Save();
		}

		public async Task UpdateAsyn(TEntity entity)
		{
			repository.Update(entity);
			await SaveAsyn();
		}

		void IDisposable.Dispose()
		{
			unitOfWork.Dispose();
		}
	}
}
