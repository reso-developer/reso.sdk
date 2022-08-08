using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Reso.Sdk.Core.BaseConnect
{
	public class UnitOfWork : IUnitOfWork, IDisposable
	{
		private DbContext dbContext;

		public UnitOfWork(DbContext dbContext)
		{
			this.dbContext = dbContext;
		}

		public int Commit()
		{
			return dbContext.SaveChanges();
		}

		public Task<int> CommitAsync()
		{
			return dbContext.SaveChangesAsync();
		}

		public virtual void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (disposing && dbContext != null)
			{
				dbContext.Dispose();
				dbContext = null;
			}
		}
	}
}
